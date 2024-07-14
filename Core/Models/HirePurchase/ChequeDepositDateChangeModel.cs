namespace BSOL.Core.Models.HirePurchase
{
    public class ChequeDepositDateChangeModel
    {
        public long ID { get; set; }
        public string ChequeNo { get; set; }
        public string Cust_Name_EN { get; set; }
        public string AgrementRef { get; set; }
        public DateTime OldDepositDate { get; set; }
        public DateTime NewDepositDate { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public decimal? Amount { get; set; }
    }
}
