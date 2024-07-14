using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Models.Accounts
{
    public class ContractExpiredList
    {
        #region columns
        public DateTime ContractFrom { get; set; }
        public DateTime ContractTo { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public int NotifyDays { get; set; }
        public string NotifyTo { get; set; }
        public string ProcessedBy { get; set; }
        public DateTime ProcessedOn { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime VerifiedDate { get; set; }
        #endregion

    }
}
