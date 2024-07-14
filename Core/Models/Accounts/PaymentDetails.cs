using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Models.Accounts
{
    public class PaymentDetails
    {
        public long ID { get; set; }
        public long PaymentMasterId { get; set; }
        public string Reference { get; set; }
        public long? ReferenceId { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal Amount { get; set; }
        public string ExpenseCode { get; set; }
        public string Details { get; set; }
        public string ReferenceNo { get; set; }
        public decimal? MaxAmount { get; set; }
        public long? GSTSettingId { get; set; }
        public decimal? GSTPercent { get; set; }

        //[NotMapped]
        public decimal TotalAmount { get; set; }
    }
}
