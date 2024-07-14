namespace BSOL.Core.Models.Accounts
{
    public class PaymentReturnModel
    {
        public long ID { get; set; }
        public string Type { get; set; }
        public string Division { get; set; }
        public string Destination { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int? Duration { get; set; }
        public string? Currency { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Balance { get; set; }
        public decimal? ReturnAmount { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ReceivedBy { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string? HandOverTo { get; set; }
        public DateTime? HandOverDate { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedDate { get; set; }
        public string PaymentType { get; set; }
        public long PaymentID { get; set; }
        public string RefNo { get; set; }
        public string? PaidBy { get; set; }
        public DateTime? PaidDate { get; set; }
    }
}
