using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Models.Procurement
{
    public class ImportedAdjustmentModel
    {
        public long Id { get; set; }
        public long SKU { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public decimal Stock { get; set; }
        public decimal Qty { get; set; }
        public string Reason { get; set; }
        public decimal PurchasedRate { get; set; }
        public string SKUFormatted { get; set; }
        public string Unit { get; set; }
        public decimal Conversion { get; set; }
    }
}
