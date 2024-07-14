using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Models.Logitics
{
    public class LorryPaymentModel
    {
        public string? Product { get; set; }
        public string? ShipmentRef { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan TotalTime { get; set; }
        public string? LorryNumber { get; set; }
        public bool IsFreeLorryBundle { get; set; }
        public int NoOfFreeBundles { get; set; }
        public int NoOfBundles { get; set; }
        public decimal ExtraBundleRate { get; set; }
        public decimal BundlesRate { get; set; }
        public decimal LorryRate { get; set; }
        public decimal RatePerMins { get; set; }
        public string? EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public long ID { get; set; }
        public long ShipmentID { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public int ExtraBundle { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? VehicleType { get; set; }
        public decimal SubsequentRatePerMins { get; set; }
        public decimal SubsequentNewLorryRate { get; set; }
        public TimeSpan NewLorryRateFrom { get; set; }
        public TimeSpan NewLorryRateTo { get; set; }
        public string? FloatName { get; set; }
        public long FloatSettingID { get; set; }
    }
}
