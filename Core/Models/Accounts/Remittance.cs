namespace BSOL.Core.Models.Accounts
{
    public class Remittance
    {
        public long ID { get; set; }
        public long ShopGroupID { get; set; }
        public string TransactionID { get; set; }
        public DateTime TransactionOn { get; set; }
        public string CustomerName { get; set; }
        public string NicNo { get; set; }
        public string ContactNo { get; set; }
        public string FromBank { get; set; }
        public string FromAccountName { get; set; }
        public string FromAccountNo { get; set; }
        public long ToBankID { get; set; }
        public string ToBankName { get; set; }
        public string ToAccountNo { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string ShopGroupViberNo { get; set; }
        public string Remarks { get; set; }
        public string AccountsRemarks { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string ReconciliationBy { get; set; }
        public DateTime? ReconciliationOn { get; set; }
        public long? CustomerID { get; set; }
        public long? ToAccountId { get; set; }
        public long? FromBankID { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
