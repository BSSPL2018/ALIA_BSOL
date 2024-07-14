using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOL.Core.Models.Procurement
{
    public class ItemCostingMasterModel
    {
        public long? ShipmentId { get; set; }
        public long? PurchaseOrderId { get; set; }
        public string RefNo { get; set; }
        public decimal? TotalValue { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? VerifiedOn { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ActualArrivalDate { get; set; }
        public DateTime? ClearedDate { get; set; }
        public string ClearanceType { get; set; }
        public string ShipmentMode { get; set; }
        public string CustomsRefNo { get; set; }
    }
}
