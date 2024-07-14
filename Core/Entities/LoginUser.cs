namespace BSOL.Core.Entities
{
    public class LoginUser
    {
        public long ID { get; set; }
        public long EmployeeID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int GroupID { get; set; }
        public int InvalidAttempts { get; set; }
        public bool Locked { get; set; }
        public bool EnforcePasswordChange { get; set; }
        public DateTime? LastPasswordChanged { get; set; }
        public string? DeviceType { get; set; }
        public string? DeviceID { get; set; }
        public string? Browser { get; set; }
        public string? BrowserID { get; set; }
        public string? LastAccessInfo { get; set; }
        public DateTime? LastAccess { get; set; }
        public string? ResetLinkID { get; set; }
        public DateTime? ResetLinkExpiry { get; set; }
        public string? DeviceInfo { get; set; }
        public string? EntryBy { get; set; }
        public DateTime? EntryDate { get; set; }
        public int? PIN { get; set; }
        public string? BSOLBrowserID { get; set; }
    }
}
