using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Models.Shipment
{
    public class ShipmentPOSerialModel
    {
        public long? ShipmentId { get; set; }
        public long ItemCostingId { get; set; }
        public string ShipmentRefNo { get; set; }
        public string PORefno { get; set; }
    }
}
