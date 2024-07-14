using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class HSNSetting : BaseObject
    {
        public long Id { get; set; }
        public string HSNCategory { get; set; }
        public string HSNCode { get; set; }
        public decimal MFN { get; set; }
        public bool IsDefault { get; set; }
        public string EntryBy { get; set; }
        public DateTime? EntryDate { get; set; }
        protected override async Task Add()
        {
            _Webcontext.HSNSettings.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Validate()
        {
            if (await _Webcontext.HSNSettings.AnyAsync(x => x.HSNCode == this.HSNCode && x.Id != this.Id))
                AddMessage("HSNCode (" + this.HSNCode + ") already exists");
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.HSNSettings.FindAsync(Id);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("HSNCode", this.Id, this.HSNCode);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task CheckDependency()
        {
            if (await _Webcontext.Items.AnyAsync(x => x.HSNId == this.Id))
                AddMessage("HSN configured in Items");
        }
        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("HSNCode", this.Id, this.HSNCode);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
