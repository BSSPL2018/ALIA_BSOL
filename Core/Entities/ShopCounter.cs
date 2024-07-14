using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class ShopCounter : BaseObject
    {
        public long Id { get; set; }
        public long ShopId { get; set; }
        public string CounterName { get; set; }
        public int NoOfCounter { get; set; }
        [NotMapped]
        public string ShopName { get; set; }

        protected override async Task Add()
        {
            _Webcontext.ShopCounters.Add(this);
            //LogAdd("Item");
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task Validate()
        {
            if (await _Webcontext.ShopCounters.AnyAsync(x => x.ShopId == this.ShopId && x.Id != this.Id))
                AddMessage("Same shop (" + this.CounterName + ") already exists");

        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.ShopCounters.FindAsync(Id);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("ShopCounters", this.CounterName, this.ShopName);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("ShopCounters", this.CounterName, this.ShopName);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
