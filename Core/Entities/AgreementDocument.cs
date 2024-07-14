using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class AgreementDocument
    {
        public long PID { get; set; }
        public string FileName { get; set; }
        public string Refno { get; set; }
        public string Details { get; set; }
        public string Category { get; set; }
        public string LocationPath { get; set; }
        public long? FID { get; set; }
        public string Module { get; set; }
        public string EntryBy { get; set; }
        public DateTime? EntryDate { get; set; }
        public string Verified { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public bool? Cancel { get; set; }
        public string Emailed { get; set; }
        public DateTime? EmailedDate { get; set; }
        public string Printed { get; set; }
        public DateTime? InvoiceDate { get; set; }
    }
}
