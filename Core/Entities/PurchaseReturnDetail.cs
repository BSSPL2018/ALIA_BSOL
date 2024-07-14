namespace BSOL.Core.Entities
{
    public class PurchaseReturnDetail
    {
        public long PurchaseReturnMasterId { get; set; }
        public long PurchaseOrderDetailId { get; set; }
        public decimal ReturnedQty { get; set; }
        public decimal ActualReturnedQty { get; set; }
    }
}
