namespace BSOL.Core.Entities
{
    public class TravelAgent
    {
        public long ID { get; set; }
        public long CompanyID { get; set; }
        public string AgencyName { get; set; }
        public string ContactName { get; set; }
        public string ContactNo { get; set; }
        public string EmailID { get; set; }
        public string TINNO { get; set; }
        public string ChequeAccountName { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
    }
}
