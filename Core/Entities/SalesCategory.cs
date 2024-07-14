using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class SalesCategory : BaseObject
    {
        #region Columns
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        #endregion

        protected override async Task Validate()
        {
            if (await _Webcontext.SalesCategories.AnyAsync(x => x.CompanyId == this.CompanyId && x.Category == this.Category && x.Id != this.Id))
                this.AddMessage("Category (" + this.Category + ") already exists");
        }
        protected override async Task Add()
        {
            _Webcontext.SalesCategories.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.SalesCategories.FindAsync(Id);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            _Webcontext.Entry(existing).Property(x => x.CreatedBy).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.CreatedOn).IsModified = false;
            LogUpdate("SALES CATEGORY", this.Category);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task CheckDependency()
        {
            if (await _Webcontext.Items.AnyAsync(x => x.SalesCategoryId == this.Id))
                this.AddMessage("Items added for this Sales Category.");
        }
        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("SALES CATEGORY", this.Category);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
