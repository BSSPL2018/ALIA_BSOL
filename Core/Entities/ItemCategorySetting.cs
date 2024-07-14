
using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class ItemCategorySetting:BaseObject
    {
        #region columns
        public long ID { get; set; }
        public long ItemCategoryId { get; set; }
        public string OrderCategory { get; set; }
        public bool ABCRanking { get; set; }
        public bool AnnualScheduledOrder { get; set; }
        public string OrderFrequency { get; set; }
        public int StockHoldingMonths { get; set; }
        public int InitialOrderQty { get; set; }
        public decimal GST { get; set; }
        public bool Colour { get; set; }
        public int SHRankA { get; set; }
        public int SHRankB { get; set; }
        public int SHRankC { get; set; }
        public int SHRankD { get; set; }
        public int OrderMinimum { get; set; }
        #endregion 

        [NotMapped]
        public string ItemCategory { get; set; }

        protected async override Task Add()
        {
            _Webcontext.ItemCategorySettings.Add(this);
            await _Webcontext.SaveChangesAsync();
        }

        protected async override Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("ITEM CATEGORY SETTING", this.ItemCategory, this.OrderCategory);
            await _Webcontext.SaveChangesAsync();
        }

        protected async override Task Update()
        {
            var existing = await _Webcontext.ItemCategorySettings.FindAsync(ID);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("ITEM CATEGORY SETTING", this.ItemCategory, this.OrderCategory);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
