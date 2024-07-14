using System;

namespace BSOL.Core.Models.Procurement
{
    public class BinCardModel
    {
        public DateTime? TransactionDate { get; set; }
        public string Process { get; set; }
        public decimal Qty { get; set; }
        public decimal Stock { get; set; }
        public string ReferenceNo { get; set; }
    }
}
