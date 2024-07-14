using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class PurchaseReturnMaster
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
        public bool PartialReturn { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
