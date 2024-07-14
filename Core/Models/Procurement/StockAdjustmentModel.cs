using BSOL.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Models.Procurement
{
    public class StockAdjustmentModel:StockAdjustment
    {
        public List<StockAdjustmentDetail> StockAdjustmentDetails { get; set; }
    }
}
