namespace BSOL.Core.Models.Common
{
    public class User
    {
        public long UserID { get; set; }
        public long EmployeeID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmpID { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public string Designation { get; set; }
        public string ImagePath { get; set; }
        public string DeviceID { get; set; }
        public bool Locked { get; set; }
        public bool EnforcePasswordChange { get; set; }
        public bool? IsAdmin { get; set; }
        public bool IsPowerUser { get; set; }
        public bool? IsEntityIncharge { get; set; }
        public bool? IsDivisionIncharge { get; set; }
        public bool? IsDepartmentIncharge { get; set; }
        public bool? IsSectionIncharge { get; set; }
        public bool? IsSuperior { get; set; }
        public bool? IsHR { get; set; }
        public bool? IsHRHead { get; set; }
        public int ViewMode { get; set; }
        public long EntityID { get; set; }
        public string EntityName { get; set; }
        public string Code { get; set; }
        public long CompanyId { get; set; }
        public string CompanyCode { get; set; }
        public string UniqueID { get; set; }
        public string PrimaryCurrency { get; set; }
        public string SecondaryCurrency { get; set; }
        public string AndroidVersion { get; set; }
        public string IOSVersion { get; set; }
        public string AndroidID { get; set; }
        public string IOSID { get; set; }
        public string OneSignalAppID { get; set; }
        public string OneSignalAppKey { get; set; }
        public string GoogleProjectNo { get; set; }
        public bool WebNotification { get; set; }
        public bool HalfDayLeave { get; set; }
        public int Level { get; set; }
        public bool? IsFrontOffice { get; set; }
        public decimal? UserSalary { get; set; }
        public string GroupName { get; set; }
        public bool Active { get; set; }
        public long ID { get; set; }
        public decimal ConversionRate { get; set; }
        public bool? IsGuest { get; set; }
        public long EmployeeId { get; set; }
        public long? ShopId { get; set; }
        public string ShopName { get; set; }
        public decimal ShopGST { get; set; }
        public long? ShopGSTInAccountId { get; set; }
        public long? ShopGSTOutAccountId { get; set; }
        public int ShopCount { get; set; }
        public string ShopCode { get; set; }
        public decimal ServiceCharge { get; set; }
        public string BaseCurrency { get; set; }
    }
}
