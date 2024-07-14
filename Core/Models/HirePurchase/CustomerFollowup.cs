using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Models.HirePurchase
{
    public class CustomerFollowup
    {
        public string? FollowupType { get; set; }
        public DateTime? FollowupDate { get; set; }
        public string? Subject { get; set; }
        public string? Details { get; set; }
        public string? Reference_No { get; set; }
        public string? Sender { get; set; }
        public string? Receiver { get; set; }
        public string? CallType { get; set; }
        public string? Remarks { get; set; }
        public bool? Cancel_Flag { get; set; }
        public DateTime? Reminder { get; set; }
        public string? InvoiceNo { get; set; }
        public string? REGNO { get; set; }
        public string? ReminderStatus { get; set; }
    }
}
