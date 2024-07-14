using System;

namespace BSOL.Core.Models.Shipment
{
    public class SerialedItemModel
    {
        public string Category { get; set; }
        public string ItemCode { get; set; }
        public string ModelCode { get; set; }
        public string Color { get; set; }
        public long ItemId { get; set; }
        public string SerialNo { get; set; }
        public string Description { get; set; }
        public long ItemCostingId { get; set; }
        public long? Id { get; set; }
    }
}
