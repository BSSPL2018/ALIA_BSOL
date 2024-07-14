namespace BSOL.Core.Models.Accounts
{
    public class RemittanceDetailsModel
    {
        public string ReceiptNo { get; set; }
        public decimal? PaidAmount { get; set; }
        public DateTime? PaidDate { get; set; }
        public string InvoiceNo { get; set; }
        public string Paidby { get; set; }
        public string EntryOutlet { get; set; }
        public string ReceiptTo { get; set; }
        public string TransactionID { get; set; }
    }
}
