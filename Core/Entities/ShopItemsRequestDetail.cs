using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class ShopItemsRequestDetail
    {
        public long Id { get; set; }
        public long ShopItemsRequestId { get; set; }
        public long RequestedItemId { get; set; }
        public decimal RequestedQty { get; set; }
        public decimal ConfirmedQty { get; set; }
        public string Remarks { get; set; }
        [NotMapped]
        public string Description { get; set; }
        [NotMapped]
        public long RequestedFromShopId { get; set; }
        [NotMapped]
        public long RequestedShopId { get; set; }
        [NotMapped]
        public long SKU { get; set; }
        [NotMapped]
        public long? UPC { get; set; }
        [NotMapped]
        public string ItemCode { get; set; }
        [NotMapped]
        public string Category { get; set; }
        [NotMapped]
        public decimal Stock { get; set; }
        [NotMapped]
        public int SNO { get; set; }
        [NotMapped]
        public string SKUFormatted { get; set; }
        [NotMapped]
        public string Size { get; set; }
        [NotMapped]
        public string Unit { get; set; }


    }
}
