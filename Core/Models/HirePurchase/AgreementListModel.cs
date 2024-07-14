using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Models.HirePurchase
{
    public class AgreementListModel
    {
        public string AgreementRefNo { get; set; }
        public string AgreementType { get; set; }
        public DateTime AgreementDate { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public long ID { get; set; }
    }
}
