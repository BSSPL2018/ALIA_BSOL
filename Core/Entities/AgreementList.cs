using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class AgreementList
    {
        public long ID { get; set; }
        public long AgreementID { get; set; }
        public long RefNo { get; set; }
        public string AgreementType { get; set; }
        public DateTime AgreementDate { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
    }
}
