namespace BSOL.Core.Models.Accounts
{
    public class PaymentRequestValue
    {
        public long SupplierId { get; set; }
        public string Currency { get; set; }
        public string PaymentMode { get; set; }
        public decimal Amount { get; set; }
        public string ChequeName { get; set; }
        public string RefNoFormatted { get; set; }
        public long PaymentRequestID { get; set; }
    }
}
