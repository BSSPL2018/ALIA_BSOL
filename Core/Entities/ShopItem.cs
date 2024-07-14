using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class ShopItem : BaseObject
    {
        public long ShopId { get; set; }
        public long ItemId { get; set; }
        public decimal Stock { get; set; }
        public string SellingPriceMode { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal OpeningStock { get; set; }

        #region additionalcolumns
        [NotMapped]
        public string ShopName { get; set; }
        [NotMapped]
        public string ItemType { get; set; }
        [NotMapped]
        public long SKU { get; set; }
        [NotMapped]
        public string ItemCode { get; set; }
        [NotMapped]
        public long? UPC { get; set; }
        [NotMapped]
        public string Unit { get; set; }
        [NotMapped]
        public decimal PurchasedRate { get; set; }
        [NotMapped]
        public decimal SellingRate { get; set; }
        [NotMapped]
        public string ItemCategory { get; set; }
        [NotMapped]
        public string ItemDescription { get; set; }
        [NotMapped]
        public string SalesAcctName { get; set; }
        [NotMapped]
        public string PurchaseAcctName { get; set; }
        [NotMapped]
        public string InventoryAcctName { get; set; }
        
        [NotMapped]
        public bool Selected { get; set; }
        [NotMapped]
        public decimal MasterStock { get; set; }
        [NotMapped]
        public decimal AvailableStock { get; set; }
        [NotMapped]
        public string SKUFormatted { get; set; }
        
        [NotMapped]
        public string Size { get; set; }
        #endregion


        protected override Task Add()
        {
            throw new NotImplementedException();
        }

        protected override Task Update()
        {
            throw new NotImplementedException();
        }

        protected override async Task Delete()
        {
            //_context.Remove(_context.ShopItems.Where(x => x.ShopId == this.ShopId && x.ItemId == this.ItemId));
            _context.Remove(this);
            LogDelete("ShopItem", this.ShopName, this.ItemCode);
            await _context.SaveChangesAsync();
        }
    }
}
