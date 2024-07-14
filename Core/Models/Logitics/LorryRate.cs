using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Models.Logitics
{
    public class LorryRate
    {
        public decimal RatePerMins { get; set; }
        public decimal NewLorryRate { get; set; }
        public decimal SubsequentRatePerMins { get; set; }
        public decimal SubsequentNewLorryRate { get; set; }
        public int NoOfFreeBundles { get; set; }
        public decimal ExtraBundleRate { get; set; }
        public TimeSpan NewLorryRateFrom { get; set; }
        public TimeSpan NewLorryRateTo { get; set; }
        public int BeforeFirstHourRateMinimumMins { get; set; }
    }
}
