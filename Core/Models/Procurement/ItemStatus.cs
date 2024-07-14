using System;

namespace BSOL.Core.Models.Procurement
{
    public class ItemStatus
    {
        public long ItemCostingId { get; set; }
        public long ItemId { get; set; }
        public string RefNo { get; set; }
        public string Category { get; set; }
        public long SKU { get; set; }
        public long? UPC { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public decimal ConfirmedQty { get; set; }
        public decimal ReceivedQty { get; set; }
        public DateTime? ClearedDate { get; set; }
        public string ReceivedBy { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string SKUFormatted { get; set; }
        public string Unit { get; set; }
        public string Size { get; set; }
        public DateTime EntryDate { get; set; }
        public bool IsNonSerialed { get; set; }
    }
}
