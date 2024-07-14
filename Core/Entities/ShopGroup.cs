using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;

namespace BSOL.Core.Entities
{
    public class ShopGroup : BaseObject
    {
        #region Columns
        public long ID { get; set; }
        public string Name { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryOn { get; set; }
        public string GSTIN { get; set; }
        #endregion

        protected override async Task Add()
        {
            _Webcontext.ShopGroups.Add(this);
            LogUpdate("Shop Group", this.Name);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Validate()
        {
            if (await _Webcontext.ShopGroups.AnyAsync(x => x.Name == this.Name && x.ID != this.ID))
                AddMessage("Same group shop (" + this.Name + ") already exists");
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.ShopGroups.FindAsync(ID);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("Shop Group", this.Name);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task CheckDependency()
        {
        }
        protected override async Task Delete()
        {
            var existing = await _Webcontext.ShopGroups.FindAsync(ID);
            _Webcontext.Remove(existing);
            await _Webcontext.SaveChangesAsync();
            LogUpdate("Shops", this.Name);
        }
    }
}
