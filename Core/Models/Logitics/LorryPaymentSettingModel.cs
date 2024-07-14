using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Models.Logitics
{
    public class LorryPaymentSettingModel
    {
        public string? LorryType { get; set; }
        public int NoOfFreeBundles { get; set; }
        public decimal ExtraBundleRate { get; set; }
        public TimeSpan NewLorryRateFrom { get; set; }
        public TimeSpan NewLorryRateTo { get; set; }
        public decimal BeforeFirstHourRate { get; set; }
        public decimal BeforeSubsequentHourRate { get; set; }
        public decimal AfterFirstHourRate { get; set; }
        public decimal AfterSubsequentHourRate { get; set; }
        public long ID { get; set; }
        public string? EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public int BeforeFirstHourRateMinimumMins { get; set; }
    }
}
