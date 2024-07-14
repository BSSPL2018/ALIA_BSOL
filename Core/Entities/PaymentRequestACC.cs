using Microsoft.EntityFrameworkCore;

namespace BSOL.Core.Entities
{
    public class PaymentRequestACC
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
        public string RefNoFormatted { get; set; }
        public string Reference { get; set; }
        public string ReferenceId { get; set; }
    }
}
