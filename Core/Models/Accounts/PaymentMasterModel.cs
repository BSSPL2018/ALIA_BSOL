using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Models.Accounts
{
    public class PaymentMasterModel
    {
        public long ID { get; set; }
        public string ReferenceNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string Currency { get; set; }
        public string PayeeName { get; set; }
        public string Category { get; set; }
        public string PaymentType { get; set; }
        public decimal Amount { get; set; }
        public decimal GST { get; set; }
        public decimal TotalAmount { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNo { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public Boolean IsPOVisible { get; set; }
        public Boolean IsShipment { get; set; }
        public long BusinessUnitId { get; set; }
        public long SupplierId { get; set; }
        public string RefNoFormatted { get; set; }   

        public string PaidBy { get; set; }
        public DateTime? PaidOn { get; set; }
        public string Receivedby { get; set; }
        public DateTime? ReceivedOn { get; set; }
        public string RejectedBy { get; set; }
        public DateTime? RejectedOn { get; set; }
        public string HandedOverTo { get; set; }
        public string ReasonForCancel { get; set; }
        public string SupplierName { get; set; }
        public string ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public long? ParentId { get; set; }
        public string ParentRefNo { get; set; }
        public string PaymentDoneFor { get; set; }
        public Boolean Active { get; set; }
    }
}
