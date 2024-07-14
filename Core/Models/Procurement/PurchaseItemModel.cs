using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Models.Procurement
{
    public class PurchaseItemModel
    {
        public long ItemId { get; set; }
        public string Description { get; set; }
        public decimal BaseRate { get; set; }
        public decimal Rate { get; set; }
        public decimal RequestedQty { get; set; }
        public long SKU { get; set; }
        public long? UPC { get; set; }
        public string ItemCode { get; set; }
        public string SupplierCode { get; set; }
        public string ItemCategory { get; set; }
        public string ImagePath { get; set; }
        public bool GSTApplicable { get; set; }
        public decimal Stock { get; set; }
        public int TotalCount { get; set; }
        public string SKUFormatted { get; set; }
        public bool IsInventory { get; set; }
        public string PurchaseUnits { get; set; }
        public string Unit { get; set; }
        public string Size { get; set; }
        public decimal Conversion { get; set; }
    }
}
