using BSOL.Core;
using BSOL.Core.Models.Common;
using BSOL.Helpers;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace BSOL.Helpers
{
    public class AppUser
    {
        private readonly ClaimsPrincipal _user;
        private readonly HttpContext _httpContext;
        private static IHttpContextAccessor _httpContextAccessor;
        public AppUser(IHttpContextAccessor httpContextAccessor)
        {
            _user = httpContextAccessor.HttpContext.User;
            _httpContextAccessor = httpContextAccessor;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public static HttpContext Current => _httpContextAccessor.HttpContext;

        public long UserId { get { return Convert.ToInt64(_user.Identity.Name); } }
        public string EmpID { get { return _user.FindFirstValue(ClaimTypes.Name.ToString()); } }
        public string CommonEmpNo { get { return _user.FindFirstValue(ClaimType.ShortName.ToString()); } }
        public string FullName { get { return _user.FindFirstValue(ClaimType.FullName.ToString()); } }
        public bool IsPowerUser { get { return Convert.ToBoolean(_user.FindFirstValue(ClaimType.IsPowerUser.ToString())); } }
        public long UserID { get { return Convert.ToInt64(_user.FindFirstValue(ClaimType.UserID.ToString())); } }
        public long CompanyId { get { return Convert.ToInt64(_user.FindFirstValue(ClaimType.CompanyId.ToString())); } }
        public bool IsAdmin { get { return Convert.ToBoolean(_user.FindFirstValue(ClaimType.IsAdmin.ToString())); } }
        public string PrimaryCurrency { get { return _user.FindFirstValue(ClaimType.PrimaryCurrency.ToString()); } }
        public decimal ConversionRate { get { return Convert.ToDecimal(_user.FindFirstValue(ClaimType.ConversionRate.ToString())); } }
        public string UniqueID { get { return _user.FindFirstValue(ClaimType.UniqueID.ToString()); } }
        public bool IsGuest { get { return Convert.ToBoolean(_user.FindFirstValue(ClaimType.IsGuest.ToString())); } }
        public long EmployeeId { get { return Convert.ToInt64(_user.FindFirstValue(ClaimType.EmployeeId.ToString())); } }
        public long ShopId { get { return Convert.ToInt64(_user.FindFirstValue(ClaimType.ShopId.ToString())); } }
        public string ShopName { get { return _user.FindFirstValue(ClaimType.ShopName.ToString()); } }
        public decimal ShopGST { get { return string.IsNullOrEmpty(_user.FindFirstValue(ClaimType.ShopGST.ToString())) ? 0 : Convert.ToDecimal(_user.FindFirstValue(ClaimType.ShopGST.ToString())); } }
        public long? ShopGSTInAccountId { get { return string.IsNullOrEmpty(_user.FindFirstValue(ClaimType.ShopGSTInAccountId.ToString())) ? 0 : Convert.ToInt64(_user.FindFirstValue(ClaimType.ShopGSTInAccountId.ToString())); } }
        public long ShopGSTOutAccountId { get { return string.IsNullOrEmpty(_user.FindFirstValue(ClaimType.ShopGSTOutAccountId.ToString())) ? 0 : Convert.ToInt64(_user.FindFirstValue(ClaimType.ShopGSTOutAccountId.ToString())); } }
        public int ShopCount { get { return Convert.ToInt32(_user.FindFirstValue(ClaimType.ShopCount.ToString())); } }
        public string ShopCode { get { return _user.FindFirstValue(ClaimType.ShopCode.ToString()); } }
        public decimal ServiceCharge { get { return Convert.ToDecimal(_user.FindFirstValue(ClaimType.ServiceCharge.ToString())); } }
        public long EntityID { get { return Convert.ToInt64(_user.FindFirstValue(ClaimType.EntityID.ToString())); } }
        public string BaseCurrency { get { return _user.FindFirstValue(ClaimType.BaseCurrency.ToString()); } }
        public string AppUserName { get { return _user.FindFirstValue(ClaimType.AppUserName.ToString()); } }
        public async Task<UserRightModel> GetRights(string formName)
        {
            var rights = (UserRightModel)_httpContext.Items["USER_RIGHTS"];
            if (rights != null)
                return rights;

            BSOLContext context = (BSOLContext)_httpContext.RequestServices.GetService(typeof(BSOLContext));
            rights = await context.ExecuteSingleAsync<UserRightModel>("spADMIN_UserRightsGet", new { this.UserID, FormName = formName, this.IsGuest });
            _httpContext.Items.Add("USER_RIGHTS", rights);
            return rights ?? new UserRightModel();
        }

        public bool HasViewAccess(string formName)
        {
            if (IsPowerUser)
                return true;

            return (_user.FindFirstValue(ClaimType.Rights.ToString()) ?? "").Split(',').Any(x => x == formName);
        }

        public async Task<bool> HasAccess(string formName, Rights eventType = Rights.View, UserRightModel rights = null)
        {
            if (IsPowerUser)
                return true;

            rights = rights ?? await GetRights(formName);

            if (rights == null)
                return false;

            bool fullRights = rights.View && rights.Add && rights.Edit && rights.Delete && rights.Approve;

            if (eventType == Rights.Full && !fullRights)
                return false;
            else if (eventType == Rights.View && !rights.View)
                return false;
            else if (eventType == Rights.Add && !rights.Add)
                return false;
            else if (eventType == Rights.Edit && !rights.Edit)
                return false;
            else if (eventType == Rights.Modify && !(rights.Add || rights.Edit))
                return false;
            else if (eventType == Rights.Delete && !rights.Delete)
                return false;
            else if (eventType == Rights.Approve && !rights.Approve)
                return false;

            var request = _httpContext.Request;

            if (eventType == Rights.Add || eventType == Rights.Edit || eventType == Rights.Modify)
            {
                if (request.Method == "POST" && request.HasFormContentType && request.Form.Keys.Any(x => x.StartsWith("models[")))//kendo grid batch editing
                {
                    var ids = request.Form.Keys.Where(x => x.EndsWith("].ID"));
                    if (!rights.Add && ids.Any(x => Convert.ToString(request.Form[x]).ToLong() == 0))
                        return false;

                    if (!rights.Edit && ids.Any(x => Convert.ToString(request.Form[x]).ToLong() > 0))
                        return false;
                }
                else
                {
                    string id = "";
                    if (request.Method == "GET")
                        id = Convert.ToString(request.Query["ID"]);

                    else if (request.HasFormContentType)
                        id = Convert.ToString(request.Form["ID"]);

                    else if (request.ContentType == System.Net.Mime.MediaTypeNames.Application.Json)
                    {
                        request.EnableBuffering();

#pragma warning disable CA2000 // Dispose objects before losing scope
                        var jsonString = await new StreamReader(request.Body).ReadToEndAsync();
#pragma warning restore CA2000 // Dispose objects before losing scope

                        request.Body.Seek(0, SeekOrigin.Begin);

                        if (!jsonString.IsValid())
                            return true;

                        if (jsonString.StartsWith("{"))
                        {
                            var json = JObject.Parse(jsonString);
                            id = json.Value<string>("Id");
                        }
                        //else if (jsonString.StartsWith("["))//TODO: json array not yet implemented
                        //{

                        //}
                    }

                    if (!rights.Add && (!id.IsValid() || id == "0"))
                        return false;

                    if (!rights.Edit && id.IsValid() && id != "0")
                        return false;
                }
            }

            return true;
        }
    }
}
