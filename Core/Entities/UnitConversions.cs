using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class UnitConversions :BaseObject
    {
        public long ID { get; set; }
        public string FromUnit { get; set; }
        public string ToUnit { get; set; }
        public string Conversion { get; set; }


     
        protected override async Task Add()
        {
        _Webcontext.UnitConversions.Add(this);
         await _Webcontext.SaveChangesAsync();
        }
         protected override async Task Update()
        {
            var existing = await _Webcontext.UnitConversions.FindAsync(ID);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("UnitConversions", this.FromUnit,this.ToUnit,this.Conversion);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("UnitConversions", this.FromUnit, this.ToUnit, this.Conversion);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
