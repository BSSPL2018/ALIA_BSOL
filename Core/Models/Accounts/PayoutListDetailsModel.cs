namespace BSOL.Core.Models.Accounts
{
    public class PayoutListDetailsModel
    {
        public string? ExpenseCode { get; set; }
        public string? Details { get; set; }
        public decimal Amount { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal? Total { get; set; }
    }
}
