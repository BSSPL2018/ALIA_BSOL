using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class StockClubbingItem
    {
        #region Columns
        public long StockClubbingId { get; set; }
        public long ItemId { get; set; }
        public decimal Qty { get; set; }
        #endregion
        #region Additional Properties
        [NotMapped]
        public long SKU { get; set; }
        [NotMapped]
        public decimal Stock { get; set; }
        [NotMapped]
        public string ItemCode { get; set; }
        #endregion
    }
}
