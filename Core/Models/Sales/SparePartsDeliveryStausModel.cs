namespace BSOL.Core.Models.Sales
{
    public class SparePartsDeliveryStausModel
    {
        public string RefNoFormatted { get; set; }
        public string QuotationNo { get; set; }
        public string Product { get; set; }
        public string CustomerName { get; set; }
        public string ActualDeliveryTo { get; set; }
        public string VesselName { get; set; }
        public string VesselContact { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string Remarks { get; set; }
        public string RequiredDeliveryOn { get; set; }
        public long ID { get; set; }
    }
}
