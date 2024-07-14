using BSOL.Core.Models.Retail;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class QuotationDetail
    {
        #region Columns
        public long Id { get; set; }
        public long QuotationId { get; set; }
        public long ItemId { get; set; }
        public string Description { get; set; }
        public decimal BaseRate { get; set; }
        public decimal Rate { get; set; }
        public decimal Qty { get; set; }
        public decimal TotalPrice { get; set; }
        public string QuotationUnit { get; set; }
        public decimal ActualQty { get; set; }
        public decimal Conversion { get; set; }
        public decimal MinSellingRate { get; set; }

        #endregion

        #region Additional Properties
        [NotMapped]
        public long SKU { get; set; }
        [NotMapped]
        public long? UPC { get; set; }
        [NotMapped]
        public string ItemCode { get; set; }
        [NotMapped]
        public bool IsInventory { get; set; }
        [NotMapped]
        public bool GSTApplicable { get; set; }
        [NotMapped]
        public string NicNo { get; set; }

        [NotMapped]
        public List<SellingUnitModel> QuotaionUnits { get; set; }
        [NotMapped]
        public string Unit { get; set; }
        [NotMapped]
        public decimal ItemRate { get; set; }
        #endregion
    }
}
