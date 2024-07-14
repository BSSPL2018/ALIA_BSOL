using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class AdvanceBooking
    {
        public long BookingID { get; set; }
        public string Product { get; set; }
        public string ItemCode { get; set; }
        public double? AdvanceAmount { get; set; }
        public DateTime? Availabledate { get; set; }
        public string BookedBy { get; set; }
        public DateTime? BookedDate { get; set; }
        public bool? CancelFlag { get; set; }
        public string CustomerRefNo { get; set; }
        public string Atoll { get; set; }
        public string Island { get; set; }
        public string Zone { get; set; }
        public string Remarks { get; set; }
        public string ProInv_No { get; set; }
        public int? NoOfQuantity { get; set; }
        public string POInvoiceNo { get; set; }
        public string Type { get; set; }
        public string StickerNo { get; set; }
    }
}
