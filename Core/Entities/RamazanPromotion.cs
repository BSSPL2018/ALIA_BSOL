namespace BSOL.Core.Entities
{
    public class RamazanPromotion
    {
        public long ID { get; set; }
        public long? OutletID { get; set; }
        public string NicNo { get; set; }
        public string CustomerName { get; set; }
        public string CouponCode { get; set; }
    }
}
