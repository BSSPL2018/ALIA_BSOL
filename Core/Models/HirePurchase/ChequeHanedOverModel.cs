namespace BSOL.Core.Models.HirePurchase
{
    public class ChequeHanedOverModel
    {
        public string CustomerName { get; set; }
        public string AgrementRef { get; set; }
        public string ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public DateTime? DepositDate { get; set; }
        public string HandedOverBy { get; set; }
        public DateTime? HandedOverOn { get; set; }
        public long ID { get; set; }
        public decimal Amount { get; set; }
        public string BankName { get; set; }
        public string EntryBy { get; set; }
        public DateTime? EntryDate { get; set; }
    }
}
