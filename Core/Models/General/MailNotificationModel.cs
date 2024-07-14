namespace BSOL.Core.Models.General
{
    public class MailNotificationModel
    {
        public string EmailSubject { get; set; }
        public string EmailTemplate { get; set; }
        public string NotificationTemplate { get; set; }
        public string SMSTemplate { get; set; }
        public string EmailID { get; set; }
        public string DeviceID { get; set; }
        public string BrowserID { get; set; }
        public string PhoneNo { get; set; }
        public string OneSignalAppID { get; set; }
        public string OneSignalAppKey { get; set; }
        public bool Mobile { get; set; }
        public bool Web { get; set; }
        public bool Email { get; set; }
    }
}
