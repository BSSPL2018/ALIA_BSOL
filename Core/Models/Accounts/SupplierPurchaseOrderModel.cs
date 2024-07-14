namespace BSOL.Core.Models.Accounts
{
    public class SupplierPurchaseOrderModel
    {
        public long PurchaseOrderId { get; set; }
        public string PORefNo { get; set; }
        public decimal Balance { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal GSTPercent { get; set; }
        public long? PaymentMasterId { get; set; }
        public long? PaymentDetailsId { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string Mode { get; set; }
        public long? GSTSettingId { get; set; }
        public long? ParentId { get; set; }
        public string PaymentType { get; set; }
        public string Currency { get; set; }
        public decimal ActualInvoiceAmount { get; set; }
    }
}
