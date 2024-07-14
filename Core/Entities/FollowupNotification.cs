using BSOL.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class FollowupNotification
    {
        public long Id { get; set; }
        [ModelBinder(typeof(IdentityDecryptor))]
        public long? CustomerId { get; set; }
        [ModelBinder(typeof(IdentityDecryptor))]
        public long? QuotationId { get; set; }
        [ModelBinder(typeof(IdentityDecryptor))]
        public long? InvoiceId { get; set; }
        [ModelBinder(typeof(IdentityDecryptor))]
        public long? ProformaInvoiceId { get; set; }
        public long EmployeeId { get; set; }
        [NotMapped]
        public bool AllowNotification { get; set; }
        [NotMapped]
        public string Employees { get; set; }
        [NotMapped]
        public bool DeleteFollower { get; set; }
    }
}
