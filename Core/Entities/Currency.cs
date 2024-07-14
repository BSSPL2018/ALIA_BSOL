using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class Curency : BaseObject
    {
        public long Id { get; set; }
        public string Currency { get; set; }
        public string DecimalWord { get; set; }
        public decimal ConversionRate { get; set; }
        protected override async Task Add()
        {
            _Webcontext.Currencies.Add(this);
            //LogAdd("Item");
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Validate()
        {
            if (await _Webcontext.Currencies.AnyAsync(x => x.Currency == this.Currency && x.Id != this.Id))
                AddMessage("Currency (" + this.Currency + ") already exists");
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.Currencies.FindAsync(Id);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("Currency", this.Id, this.Currency);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("Currency", this.Id, this.Currency);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
