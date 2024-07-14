namespace BSOL.Core.Models.Accounts
{
    public class SettlementDetailsModel
    {
        public string InvoiceNo { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime PaidDate { get; set; }
        public string Paidby { get; set; }
        public decimal PaidAmount { get; set; }
        public string Receipt_Category { get; set; }
        public string PaymentMode { get; set; }
        public string Cheque_No { get; set; }
        public string ShopName { get; set; }
    }
}
