using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class SupplierInvoiceDetail
    {
        public long SupplierInvoiceId { get; set; }
        public long PurchaseOrderId { get; set; }
        public decimal InvoiceAmount { get; set; }
        [NotMapped]
        public string PORefNo { get; set; }
        [NotMapped]
        public decimal TotalAmount { get; set; }
        [NotMapped]
        public decimal Balance { get; set; }
    }

}
