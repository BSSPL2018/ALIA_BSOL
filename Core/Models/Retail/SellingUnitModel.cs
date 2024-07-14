using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOL.Core.Models.Retail
{
    public class SellingUnitModel
    {
        public string SellingUnit { get; set; }
        public long Id { get; set; }
        public decimal SellingUnitRate { get; set; }
        public decimal Conversion { get; set; }
        public long SellingUnitId { get; set; }
    }
}
