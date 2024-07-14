using BSOL.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class PaymentRequest
    {
        public long ID { get; set; }
        public string Category { get; set; }
        public int RefNo { get; set; }
        public string PageRefNo { get; set; }
        public string PaymentType { get; set; }
        public string Currency { get; set; }
        public decimal TotalAmount { get; set; }
        public int PaymentCount { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ReceivedBy { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string PaidBy { get; set; }
        public DateTime? PaidDate { get; set; }
        public string HandOverTo { get; set; }
        public DateTime? HandOverDate { get; set; }
        public string VoucherNo { get; set; }
        public string ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string ProcessName { get; set; }
        public string Department { get; set; }
        public string Reference { get; set; }
        public string ReferenceId { get; set; }
        public bool? IsPayout { get; set; }
        public decimal? ReturnAmount { get; set; }
        public long? CreditTo { get; set; }
        public long? DebitTo { get; set; }
        public string RefNoFormatted { get; set; }
        public string PayeeName { get; set; }
        public long? BankId { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNo { get; set; }
        public string PaymentCancelledBy { get; set; }
        public DateTime? PaymentCancelledDate { get; set; }
        public long? ParentId { get; set; }
        public bool? Active { get; set; }
        public string RejectedBy { get; set; }
        public DateTime? RejectedOn { get; set; }
        public string ReasonForCancel { get; set; }
        public string ReferenceNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? GST { get; set; }
        public long? SupplierId { get; set; }
        public long? BusinessUnitId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? GSTPercent { get; set; }
        public decimal? ConversionRate { get; set; }
        public string Remarks { get; set; }
        public long? StaffId { get; set; }
        public long? CustomerId { get; set; }
        public string TransactionNo { get; set; }
        public DateTime? TransactionDate { get; set; }

        [NotMapped]
        public decimal Balance { get; set; }
        [NotMapped]
        public decimal AmountWOGST { get; set; }
        [NotMapped]
        public string SupplierName { get; set; }

    }
}
