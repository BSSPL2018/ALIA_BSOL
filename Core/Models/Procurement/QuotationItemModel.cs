using BSOL.Core.Models.Retail;

namespace BSOL.Core.Models.Procurement
{
    public class QuotationItemModel
    {
        public long ItemId { get; set; }
        public long SKU { get; set; }
        public long? UPC { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string ItemType { get; set; }
        public decimal Rate { get; set; }
        public decimal ShopStock { get; set; }
        public decimal MasterStock { get; set; }
        public bool IsInventory { get; set; }
        public bool GSTApplicable { get; set; }
        public string CustomerCode { get; set; }
        public string SKUFormatted { get; set; }
        public List<SellingUnitModel> QuotaionUnits { get; set; }

        public string Unit { get; set; }
        public decimal Conversion { get; set; }
        public decimal ItemRate { get; set; }
        public decimal? MinSellingRate { get; set; }
    }
}
