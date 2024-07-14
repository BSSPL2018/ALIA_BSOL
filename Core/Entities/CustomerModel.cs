
namespace BSOL.Core.Entities
{
    public class CustomerModel 
    {
        #region Columns
        public long ID { get; set; }
        public string NicNo { get; set; }
        public string NameEN { get; set; }
        public string NameDHI { get; set; }
        public string PermanentAddressEN { get; set; }
        public string PermanentAddressDHI { get; set; }
        public string PresentAddressEN { get; set; }
        public string PresentAddressDHI { get; set; }
        public string ContactNo { get; set; }
        public string MobileNo { get; set; }
        public string EmailID { get; set; }
        public string FaxNo { get; set; }
        public string TINNo { get; set; }
        public int? WorkedMonth { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public string CustomerType { get; set; }
        public string EmployerType { get; set; }
        public string EmployerName { get; set; }
        public string EmployerDetails { get; set; }
        public string EmployerPresentAddressEN { get; set; }
        public string EmployerPresentAddressDHI { get; set; }
        public string EmployerContactNo { get; set; }
        public string EmergencyContactNameEN { get; set; }
        public string EmergencyContactNameDHI { get; set; }
        public string EmergencyContactNo { get; set; }
        public string Remarks { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public bool Active { get; set; }
        #endregion

    }
}
