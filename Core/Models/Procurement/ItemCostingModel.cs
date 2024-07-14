namespace BSOL.Core.Models.Procurement
{
    public class ItemCostingModel
    {
        public long? ShipmentId { get; set; }
        public long? PurchaseOrderId { get; set; }
        public long ItemId { get; set; }
        public long SKU { get; set; }
        public string SKUFormatted { get; set; }
        public string? ItemCode { get; set; }
        public string? Unit { get; set; }
        public string? Size { get; set; }
        public string? Description { get; set; }
        public string Currency { get; set; }
        public decimal OldQty { get; set; }
        public decimal NewQty { get; set; }
        public decimal CustomDutyFee { get; set; }
        public decimal Freight { get; set; }
        public decimal OtherExpenses { get; set; }
        public decimal? TotalExpenses { get; set; }
        public decimal OldCostOfGoods { get; set; }
        public decimal PurchasedRate { get; set; }
        public decimal NewCostOfGoods { get; set; }
        public decimal WAC { get; set; }
        public decimal? TotalCost { get; set; }
        public decimal? Profit { get; set; }
        public decimal OldSellingRate { get; set; }
        public decimal NewSellingRate { get; set; }
        public string? HSNCode { get; set; }
        public decimal HSNPercentage { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? VerifiedBy { get; set; }
        public DateTime? VerifiedOn { get; set; }
    }
}
