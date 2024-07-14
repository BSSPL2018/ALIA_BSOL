namespace BSOL.Core.Models.DashBoard
{
    public class SalesStatus
    {
        public long? SNO { get; set; }
        public string Unit { get; set; }
        public string Cust_Name_EN { get; set; }
        public string Cust_Ref { get; set; }
        public string ItemCode { get; set; }
        public string Frameno { get; set; }
        public string Registration_No { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? BalanceAmount { get; set; }
        public string InvoiceNo { get; set; }
        public string Invoice_Date { get; set; }
        public string EntryBy { get; set; }
        public string InvoiceType { get; set; }
        public string SalesType { get; set; }
        public string SoldBy { get; set; }
        public decimal? Paid_Amount { get; set; }
        public string Cust_MobileNo { get; set; }
        public string CustomerType { get; set; }
        public decimal WriteOff_AMt { get; set; }
        public DateTime? LFD { get; set; }
        public decimal? StaffPrice { get; set; }
        public int PaymntSts { get; set; }
        public decimal? Default_Amt { get; set; }
        public decimal? InitialAmount { get; set; }
        public int? No_Payment { get; set; }
        public string PType { get; set; }
        public string PromotionCode { get; set; }
        public string AgrementRef { get; set; }
        public string ProformaNo { get; set; }
    }
}
