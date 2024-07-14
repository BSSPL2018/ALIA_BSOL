namespace BSOL.Core.Models.HirePurchase
{
    public class ChequeListModel
    {
        public long ID { get; set; }
        public string Cust_Name_EN { get; set; }
        public long CustomerID { get; set; }
        public long BankID { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNo { get; set; }
        public string ChequeNo { get; set; }
        public decimal Amount { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public DateTime? SentToBank { get; set; }
        public Boolean? IsSelect { get; set; }
        public string Currency { get; set; }
        public decimal Balance { get; set; }
        public string BounceUpdatedBy { get; set; }
        public DateTime? BounceUpdatedOn { get; set; }
        public string ClearedBy { get; set; }
        public DateTime? ClearedOn { get; set; }
        public string DepositedBy { get; set; }
        public DateTime? DepositedOn { get; set; }
        public string IsChequeExpire { get; set; }
        public string DepositedBank { get; set; }
        public string DepositedAccountNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string Remarks { get; set; }
        public decimal Settled { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public long? AgreementID { get; set; }
        public string AgrementRef { get; set; }
        public string BounceRemarks { get; set; }
        public string IsChequePrinted { get; set; }

        public string LockerName { get; set; }
        public int RackNo { get; set; }
        public int NoofBin { get; set; }
        public int NoofLevel { get; set; }
        public string InvoiceNo { get; set; }

        public string selectColumn { get; set; }
        public string CustomerChangeColumn { get; set; }
        public string DepositedColumn { get; set; }
        public string ChequeStatus { get; set; }
    }
}
