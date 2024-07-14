namespace BSOL.Core.Models.HirePurchase
{
    public class ChequeList
    {
        public long ID { get; set; }
        public long CustomerID { get; set; }
        public long? AgreementID { get; set; }
        public long BankID { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNo { get; set; }
        public string ChequeNo { get; set; }
        public decimal Amount { get; set; }
        public DateTime? SentToBank { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public bool Active { get; set; }
        public string Currency { get; set; }
        public string DepositRemarks { get; set; }
        public string DepositedBy { get; set; }
        public string Remarks { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
    }
}
