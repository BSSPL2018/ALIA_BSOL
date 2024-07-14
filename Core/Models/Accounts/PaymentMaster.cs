using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Models.Accounts
{
    public class PaymentMaster
    {
        public long ID { get; set; }
        public long RefNo { get; set; }
        public long SupplierId { get; set; }
        public string BusinessUnitId { get; set; }
        public string PayeeType { get; set; }
        public string PaymentCategory { get; set; }
        public string ReferenceNo { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal GST { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal GSTPercent { get; set; }
        public decimal ConversionRate { get; set; }
        public string PaymentMode { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNo { get; set; }
        public string PayeeName { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string Receivedby { get; set; }
        public DateTime? ReceivedOn { get; set; }
        public string PaidBy { get; set; }
        public DateTime? PaidOn { get; set; }
        public string HandedOverTo { get; set; }
        public string RejectedBy { get; set; }
        public DateTime? RejectedOn { get; set; }
        public string ReasonForCancel { get; set; }
        public bool Active { get; set; }
        public string RefNoFormatted { get; set; }
        public string Remarks { get; set; }
        public long? BankId { get; set; }
        public long? DebitTo { get; set; }
        public long? CreditTo { get; set; }
        public long? ReferenceId { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public long? ParentId { get; set; }

        [NotMapped]
        public Boolean IsPOVisible { get; set; }
        [NotMapped]
        public Boolean IsShipment { get; set; }
        [NotMapped]
        public string SupplierName { get; set; }
        [NotMapped]
        public decimal InvoiceAmount { get; set; }
        [NotMapped]
        public decimal Balance { get; set; }
        [NotMapped]
        public decimal AmountWOGST { get; set; }
        [NotMapped]
        public long? PurchaseOrderId { get; set; }
    }
}
