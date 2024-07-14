using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class SerialedItem
    {
        #region Columns
        public long Id { get; set; }
        public long? ItemCostingId { get; set; }
        public long ItemId { get; set; }
        public string SerialNo { get; set; }
        public decimal Qty { get; set; }
        public decimal Stock { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string ReceivedBy { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public decimal ActualQty { get; set; }
        #endregion

        #region Additional Properties
        [NotMapped]
        public string ShipmentRefNo { get; set; }
        [NotMapped]
        public string ShipmentInvoiceNo { get; set; }
        [NotMapped]
        public string PurchaseOrderRefNo { get; set; }
        [NotMapped]
        public DateTime? ClearedDate { get; set; }
        [NotMapped]
        public string Category { get; set; }
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
        public string Size { get; set; }
        [NotMapped]
        public string Unit { get; set; }
        #endregion
    }
}
