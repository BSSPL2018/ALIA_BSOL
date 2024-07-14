using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class PurchaseReturn
    {
        public long Id { get; set; }
        public long SupplierInvoiceId { get; set; }
        public string ShopCode { get; set; }
        public int RefNo { get; set; }
        public DateTime ReturnedDate { get; set; }
        public decimal? GSTAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Remarks { get; set; }
        public string RefNoFormatted { get; set; }
        public string PORefNo { get; set; }
        public bool PartialReturn { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ShipmentId { get; set; }

        public string SupplierName { get; set; }
        public string InvoiceRefNoFormatted { get; set; }
        public decimal? ReturnedQty { get; set; }
        [NotMapped]
        public long ItemId { get; set; }
        [NotMapped]
        public long PurchaseOrderDetailId { get; set; }
    }
}
