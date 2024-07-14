
using BSOL.Core.Models.Retail;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class StockAdjustmentDetail
    {
        #region Columns
        public long StockAdjustmentId { get; set; }
        public long ItemId { get; set; }
        public decimal PrevQty { get; set; }
        public decimal Qty { get; set; }
        public decimal PurchasedRate { get; set; }
        public string Reason { get; set; }
        public string AdjustmentUnit { get; set; }
        public decimal ActualQty { get; set; }
        public decimal Conversion { get; set; }
		public long? SerialedItemId { get; set; }
		#endregion
		#region Additional Properties
		[NotMapped]
        public long SKU { get; set; }
        [NotMapped]
        public long? UPC { get; set; }
        [NotMapped]
        public string ItemCode { get; set; }
        [NotMapped]
        public string Description { get; set; }
        [NotMapped]
        public string SKUFormatted { get; set; }
        [NotMapped]
        public string Unit { get; set; }
        [NotMapped]
        public string Size { get; set; }
        [NotMapped]
        public List<SellingUnitModel> AdjustmentUnits { get; set; }
        #endregion
    }
}
