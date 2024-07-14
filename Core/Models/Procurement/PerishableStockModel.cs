namespace BSOL.Core.Models.Procurement
{
    public class PerishableStockModel
    {
        public long Id { get; set; }
        public string SKUFormatted { get; set; }
		public string ItemCode { get; set; }
		public string SerialNo { get; set; }
        public string Description { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal Stock { get; set; }
        public string Category { get; set; }
        public string ItemType { get; set; }
        public string ReceivedBy { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public bool IsSerialized { get; set; }
        public bool IsPerishable { get; set; }
        public string Unit { get; set; }
        public string Size { get; set; }
        public long ItemId { get; set; }
		public long? ItemCostingId { get; set; }
	}
}
