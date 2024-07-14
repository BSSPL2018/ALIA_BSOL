using System;

namespace BSOL.Core.Models.Accounts
{
    public class ChartOfAccountModel
    {
        public long Id { get; set; }
        public string AccountName { get; set; }
        public string Type { get; set; }
        public string? Code { get; set; }
        public string? Currency { get; set; }
        public decimal? OpeningBalance { get; set; }
        public DateTime? BalanceOn { get; set; }
        public long? ParentId { get; set; }
        public long ItemId { get; set; }
    }
}
