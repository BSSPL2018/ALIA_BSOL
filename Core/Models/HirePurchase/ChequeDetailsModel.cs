namespace BSOL.Core.Models.HirePurchase
{
    public class ChequeDetailsModel
    {
        public string InvoiceNo { get; set; }
        public string ReceiptNo { get; set; }
        public decimal? Paid { get; set; }
        public DateTime? PaidDate { get; set; }
        public string Receipt_Category { get; set; }
        public string PaymentFrom { get; set; }
        public string ChequeNo { get; set; }
        public string CustomerName { get; set; }
        public string Paidby { get; set; }
        public bool? IsHP { get; set; }
    }
}
