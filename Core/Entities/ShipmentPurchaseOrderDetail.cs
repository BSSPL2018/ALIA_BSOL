using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class ShipmentPurchaseOrderDetail
    {
        #region Columns
        public long ShipmentId { get; set; }
        public long PurchaseOrderDetailId { get; set; }
        public decimal ConfirmedQty { get; set; }
        public decimal ReceivedQty { get; set; }
        public string ReceivedBy { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? ReceivedOn { get; set; }
        public bool Returned { get; set; }
        #endregion

        #region Additional Properties
        [NotMapped]
        public string RefNoFormatted { get; set; }
        [NotMapped]
        public long PurchaseOrderId { get; set; }
        [NotMapped]
        public long ItemId { get; set; }
        [NotMapped]
        public long SKU { get; set; }
        [NotMapped]
        public long? UPC { get; set; }
        [NotMapped]
        public string Description { get; set; }
        [NotMapped]
        public decimal RequestedQty { get; set; }
        #endregion
    }
}
