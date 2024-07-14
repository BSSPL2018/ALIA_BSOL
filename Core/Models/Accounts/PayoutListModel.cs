namespace BSOL.Core.Models.Accounts
{
    public class PayoutListModel
    {
        public long ID { get; set; }
        public string? ReferenceNo { get; set; }
        public string? InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string Currency { get; set; }
        public string PayeeType { get; set; }
        public string PaymentCategory { get; set; }
        public string PaymentMode { get; set; }
        public decimal Amount { get; set; }
        public decimal? GST { get; set; }
        public decimal TotalAmount { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNo { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string RefNoFormatted { get; set; }
        public string? PaidBy { get; set; }
        public DateTime? PaidOn { get; set; }
        public string? HandedOverTo { get; set; }
        public string? RejectedBy { get; set; }
        public DateTime? RejectedOn { get; set; }
        public string? ReasonForCancel { get; set; }
        public string? Receivedby { get; set; }
        public DateTime? ReceivedOn { get; set; }
        public string? SupplierName { get; set; }
        public string? ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public Boolean CanReceive { get; set; }
        public Boolean UndoReceive { get; set; }
        public Boolean UndoPaid { get; set; }
        public Boolean CanReject { get; set; }
    }
}
