namespace BSOL.Core.Models.Accounts
{
    public class RemittanceModel
    {
        public long ID { get; set; }
        public string TransactionID { get; set; }
        public DateTime? TransactionOn { get; set; }
        public string FromBank { get; set; }
        public string FromAccountName { get; set; }
        public string FromAccountNo { get; set; }
        public string ToBankName { get; set; }
        public string ToAccountNo { get; set; }
        public decimal Amount { get; set; }
        public string CustomerName { get; set; }
        public string ContactNo { get; set; }
        public string NicNo { get; set; }
        public string ShopGroupViberNo { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public string OutletApprovedBy { get; set; }
        public DateTime? OutletApprovedOn { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string ReconciliationBy { get; set; }
        public DateTime? ReconciliationOn { get; set; }
        public int ToBankID { get; set; }

        public decimal? Balance { get; set; }
        public string Remarks { get; set; }
        public string AccountsRemarks { get; set; }
        public Boolean CanApprove { get; set; }
        public Boolean CanUndo { get; set; }
        public long? FromBankID { get; set; }
        public long ToAccountId { get; set; }
        public string Currency { get; set; }
        public long ShopGroupID { get; set; }
        public string ShopGroupName { get; set; }
        public long? CustomerID { get; set; }
    }
}
