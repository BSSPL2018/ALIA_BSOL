using BSOL.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class FollowUp
    {
        public long Id { get; set; }
        public string Subject { get; set; }
        public string ContactName { get; set; }
        public string ContactNo { get; set; }
        public string FollowupType { get; set; }
        public string ReferenceNo { get; set; }
        [ModelBinder(typeof(IdentityDecryptor))]
        public long ReferenceId { get; set; }
        public string Reference { get; set; }
        public string CallType { get; set; }
        public DateTime? FollowupDate { get; set; }
        public TimeSpan? FollowupTime { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public TimeSpan? ScheduledTime { get; set; }
        public string Details { get; set; }
        public string DialledBy { get; set; }
        public string Email { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        [NotMapped]
        public string EmployeeEmail { get; set; }
    }
}
