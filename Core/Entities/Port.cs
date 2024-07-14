using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class Port : BaseObject
    {
        #region Columns
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string CountryName { get; set; }
        public string PortName { get; set; }
        #endregion
        protected override async Task Add()
        {
            _Webcontext.Ports.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Validate()
        {
            if (await _Webcontext.Ports.AnyAsync(x => x.CompanyId == this.CompanyId && x.PortName == this.PortName && x.Id != this.Id))
                AddMessage("Port (" + this.PortName + ") already exists");
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.Ports.FindAsync(Id);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("Port", this.Id, this.PortName);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("Port", this.Id, this.PortName);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
