using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Models.Logitics
{
    public class LorryPayment
    {
        public long ID { get; set; }
        public string Product { get; set; }
        public long ShipmentID { get; set; }
        public string LorryNumber { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan TotalTime { get; set; }
        public bool IsFreeLorryBundle { get; set; }
        public int NoOfFreeBundles { get; set; }
        public int NoOfBundles { get; set; }
        public decimal ExtraBundleRate { get; set; }
        public decimal BundlesRate { get; set; }
        public decimal RatePerMins { get; set; }
        public decimal NewLorryRate { get; set; }
        public decimal SubsequentRatePerMins { get; set; }
        public decimal SubsequentNewLorryRate { get; set; }
        public decimal LorryRate { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        //public string EntryBy { get; set; }
        //public DateTime EntryDate { get; set; }
        public bool Active { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string VehicleType { get; set; }
        public TimeSpan NewLorryRateFrom { get; set; }
        public TimeSpan NewLorryRateTo { get; set; }
        public long FloatSettingID { get; set; }
        public int BeforeFirstHourRateMinimumMins { get; set; }

    }
}
