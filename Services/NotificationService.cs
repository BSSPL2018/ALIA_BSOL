using BSOL.Core;
using BSOL.Helpers;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Text;

namespace BSOL.Services
{
    public class NotificationService
    {
        private readonly IBackgroundTaskQueue _queue;
        private readonly AppUser _appUser;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public NotificationService(AppUser appUser, IWebHostEnvironment webHostEnvironment, IConfiguration configuration, HttpClient httpClient, IBackgroundTaskQueue queue, IServiceScopeFactory serviceScopeFactory)
        {
            _appUser = appUser;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _httpClient = httpClient;
            _queue = queue;
            _serviceScopeFactory = serviceScopeFactory;

        }

        public void Send(string module, long id, long compositeId = 0)
        {
            long AppUser = _appUser.EmployeeId;
            var baseUrl = ApplicationContext.Current != null ? string.Format("{0}://{1}", ApplicationContext.Current.Request.Scheme, ApplicationContext.Current.Request.Host.Value) : "";

            _queue.Enqueue(async (x) =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var webcontext = scope.ServiceProvider.GetRequiredService<BSOLWebContext>();
                    List<Notification> receivers = await webcontext.ExecuteSpAsync<Notification>("spHRMS_GetNotification", new { module, id, compositeId, AppUser, baseUrl });
                    Send(receivers, id);
                }
            });
        }

        private void Send(List<Notification> receivers, long id)
        {
            if (receivers.Count == 0)
                return;

            List<string> deviceIDs = new List<string>();
            Parallel.Invoke(async () => /* SMS */
            {
                foreach (Notification receiver in receivers.Where(x => x.SMS && x.PhoneNo.IsValid() && x.SMSTemplate.IsValid()))
                {
                    bool issucces = await SendSMS(receiver.PhoneNo, receiver.SMSTemplate);
                    if (issucces)
                    {
                        List<SqlParameter> param = new List<SqlParameter>
                        {
                            new SqlParameter("@Option", "SMS_LOG"),
                            new SqlParameter("@ID", id),
                            new SqlParameter("@Remarks", receiver.SMSTemplate)
                        };
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var webcontext = scope.ServiceProvider.GetRequiredService<BSOLWebContext>();
                            await webcontext.ExecuteSqlNonQueryAsync("spPOS_SparePartsRequest", param);
                        }
                    }
                }
            },
            async () => /* InAppNotifications */
            {
                var list = receivers.Where(x => (x.Mobile && x.DeviceID.IsValid()) || (x.Web && x.BrowserID.IsValid()))
                                    .GroupBy(x => x.NotificationTemplate)
                                    .Select(x => new
                                    {
                                        NotificationTemplate = x.Key,
                                        DeviceIds = x.SelectMany(y => new[] { y.DeviceID, y.BrowserID }).Where(y => y.IsValid()).ToList()
                                    });
                foreach (var item in list)
                {
                    await SendPushNotification(item.DeviceIds, item.NotificationTemplate);
                }
            });
        }

        public async Task<bool> SendPushNotification(List<string> deviceIDs, string notification)
        {
            string url = _configuration.GetValue<string>("PushNotification:Url");
            string oneSignalAppID = _configuration.GetValue<string>("PushNotification:OneSignalAppID");
            string oneSignalAppKey = _configuration.GetValue<string>("PushNotification:OneSignalAppKey");
            if (string.IsNullOrEmpty(url) || !oneSignalAppID.IsValid() || !oneSignalAppKey.IsValid() || !notification.IsValid() || !(deviceIDs ?? new List<string>()).Any())
                return false;

            if (_configuration.GetValue<string>("AppEnvironment").ToUpper() != "PRODUCTION")
            {
                deviceIDs.Clear();
                if (_configuration.GetValue<string>("PushNotification:TestDeviceID").IsValid())
                    deviceIDs.Add(_configuration.GetValue<string>("PushNotification:TestDeviceID"));

                if (_configuration.GetValue<string>("PushNotification:TestBrowserID").IsValid())
                    deviceIDs.Add(_configuration.GetValue<string>("PushNotification:TestBrowserID"));
            }

            deviceIDs = deviceIDs.Where(x => x.IsValid()).Distinct().ToList();
            if (deviceIDs.Count == 0)
                return false;

            var data = new
            {
                app_id = oneSignalAppID,
                include_player_ids = deviceIDs,
                android_led_color = "FFFFFF",
                large_icon = "icon",//TODO: not done
                small_icon = "sicon",//TODO: not done
                contents = new { en = notification }
            };
            var headers = new Dictionary<string, string> { { "Authorization", $"Basic {oneSignalAppKey}" } };

            using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_configuration.GetValue<string>("SMS:Url"))))
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                using (HttpResponseMessage result = await _httpClient.SendAsync(request))
                {
                    var response = await result.Content.ReadAsStringAsync();
                    bool isSent = false;
                    if (result.IsSuccessStatusCode)
                    {
                        if ((response ?? "").ToLower().Contains((_configuration.GetValue<string>("SMS:SuccessResponse") ?? "").ToLower()))
                            return isSent;
                    }

                    await Save(string.Join(",", deviceIDs), notification, response, isSent, MessageType.Notification);
                    return isSent;
                }
            }
        }

        public async Task<bool> SendSMS(string mobileNo, string message)
        {
            var xmlPath = Path.Combine(_webHostEnvironment.ContentRootPath, "SiteContent", "DhiraaguSingleSMS.xml");
            if (!File.Exists(xmlPath))
                throw new FileNotFoundException($"The file '{xmlPath}' is not exists.");

            if (_configuration.GetValue<string>("AppEnvironment").ToUpper() != "PRODUCTION")
                mobileNo = _configuration.GetValue<string>("SMS:TestMobileNo");

            mobileNo = mobileNo ?? "";
            mobileNo = mobileNo.Replace("+", "").Replace(" ", "").Replace("-", "");

            if (string.IsNullOrEmpty(_configuration.GetValue<string>("SMS:Url")) || mobileNo.Length < 7 || (!mobileNo.StartsWith("960") && mobileNo.Length > 7))
                return false;

            string xmlData = File.ReadAllText(xmlPath);

            xmlData = xmlData.Replace("{UserName}", _configuration.GetValue<string>("SMS:UserName"));
            xmlData = xmlData.Replace("{Password}", Utilities.Decrypt(_configuration.GetValue<string>("SMS:Password"), disableSpecialChars: true));
            xmlData = xmlData.Replace("{Message}", message);
            xmlData = xmlData.Replace("{MobileNo}", mobileNo);

            using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_configuration.GetValue<string>("SMS:Url"))))
            {
                request.Content = new StringContent(xmlData, Encoding.UTF8, "application/xml");

                using (HttpResponseMessage result = await _httpClient.SendAsync(request))
                {
                    var response = await result.Content.ReadAsStringAsync();
                    bool isSent = false;
                    if (result.IsSuccessStatusCode)
                    {
                        if ((response ?? "").ToLower().Contains((_configuration.GetValue<string>("SMS:SuccessResponse") ?? "").ToLower()))
                            isSent = true;
                    }

                    await Save(mobileNo, message, response, isSent, MessageType.SMS);
                    return isSent;
                }
            }
        }

        private async Task Save(string receiver, string messageDetails, string response, bool isSuccess, MessageType messageType)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var webcontext = scope.ServiceProvider.GetRequiredService<BSOLWebContext>();
                await webcontext.ExecuteNonQueryAsync("spHRMS_Message", new { receiver, messageDetails, response, MessageFrom = "BSOL - Web", isSuccess, MessageType = messageType.ToString() });
            }
        }
    }
}
