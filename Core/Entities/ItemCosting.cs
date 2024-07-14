namespace BSOL.Core.Entities
{
    public class ItemCosting
    {
        public long ID { get; set; }
        public long CompanyId { get; set; }
        public long? ShipmentId { get; set; }
        public long? PurchaseOrderId { get; set; }
        public long ItemId { get; set; }
        public string? Unit { get; set; }
        public decimal PurchasedRate { get; set; }
        public decimal ConfirmedQty { get; set; }
        public decimal CustomDutyFee { get; set; }
        public decimal Freight { get; set; }
        public decimal OtherExpenses { get; set; }
        public decimal CostOfGoods { get; set; }
        public decimal WAC { get; set; }
        public decimal SellingRate { get; set; }
        public decimal OldQty { get; set; }
        public decimal OldCostOfGoods { get; set; }
        public decimal OldSellingRate { get; set; }
        public double CostingPercentage { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? VerifiedBy { get; set; }
        public DateTime? VerifiedOn { get; set; }
        public decimal ReceivedQty { get; set; }
        public string? ReceivedBy { get; set; }
        public DateTime? ReceivedDate { get; set; }
    }
}
