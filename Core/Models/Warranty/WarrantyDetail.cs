using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Models.Warranty
{
    public class WarrantyDetail
    {
        public long? Warranty_ID { get; set; }
        public string? EngineNo { get; set; }
        public string? ProductCode { get; set; }
        public string? Itemcode { get; set; }
        public string? ModelCode { get; set; }
        public DateTime? Invoice_Date { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? WarrantyRegistrationDate { get; set; }
        public string? CustomerName { get; set; }
        public string? Cust_ContactNo { get; set; }
    }
}
