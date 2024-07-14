using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class UnitOfMeasures : BaseObject
    {
        public long ID { get; set; }
        public string UOM { get; set; }
      

        protected override async Task Add()
        {
            _Webcontext.UnitOfMeasures.Add(this);
            await _Webcontext.SaveChangesAsync();
        }

       
        protected override async Task Update()
        {
            var existing = await _Webcontext.UnitOfMeasures.FindAsync(ID);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("UnitOfMeasures", this.UOM);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task Delete()
        {
            //var existing = await _Webcontext.UnitOfMeasures.FindAsync(ID);
            //_Webcontext.UnitOfMeasures.Remove(existing);
            //Log("UnitOfMeasures", ActionType.Delete, this.UOM);
            //await _Webcontext.SaveChangesAsync();
            _Webcontext.Remove(this);
            LogDelete("UnitOfMeasures");
            await _Webcontext.SaveChangesAsync();
        }
    }
}
