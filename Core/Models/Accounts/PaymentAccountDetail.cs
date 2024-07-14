using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Models.Accounts
{
    public class PaymentAccountDetail
    {
        public long ID { get; set; }
        public long PaymentMasterId { get; set; }
        public long DebitTo { get; set; }
        public long CreditTo { get; set; }
        public long ShopGroupId { get; set; }
        public decimal Amount { get; set; }
        [NotMapped]
        public string DebitToName { get; set; }
        [NotMapped]
        public string CreditToName { get; set; }
    }
}
