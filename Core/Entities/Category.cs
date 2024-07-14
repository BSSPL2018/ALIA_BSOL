using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class Category : BaseObject
    {
        public long ID { get; set; }
        public long EntityID { get; set; }
        [Column("Category")]
        public string CategoryName { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }

        protected override async Task Add()
        {
            _Webcontext.Categories.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Validate()
        {
            if (await _Webcontext.Categories.AnyAsync(x => x.EntityID == this.EntityID && x.ID != this.ID && x.CategoryName == this.CategoryName))
                AddMessage("Same CategoryName (" + this.CategoryName + ") already exists");
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.Categories.FindAsync(ID);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("Category", this.CategoryName);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task CheckDependency()
        {
            if (await _Webcontext.Employments.AnyAsync(x => x.CategoryID == this.ID))
                AddMessage("Assigned to Employment Details.");
        }
        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("Category", this.CategoryName);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
