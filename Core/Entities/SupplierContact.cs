using Microsoft.EntityFrameworkCore;

namespace BSOL.Core.Entities
{
    public class SupplierContact:BaseObject
    {
        #region Columns
        public long Id { get; set; }
        public long SupplierId { get; set; }
        public string ContactName { get; set; }
        public string ContactNo { get; set; }
        public string Position { get; set; }
        public bool IsPrimaryContact { get; set; }
        public string? EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public string EmailId { get; set; }
        #endregion

        protected override async Task Validate()
        {
            if (await _Webcontext.SupplierContacts.AnyAsync(x => x.SupplierId == this.SupplierId && x.ContactName == this.ContactName && x.Id != this.Id))
                AddMessage("Same Contact (" + this.ContactName + ") already exists");
        }
        protected override async Task Add()
        {
            _Webcontext.SupplierContacts.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.SupplierContacts.FindAsync(Id);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            _Webcontext.Entry(existing).Property(x => x.EntryBy).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.EntryDate).IsModified = false;
            LogUpdate("SUPPLIER CONTACT", this.ContactName);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task CheckDependency()
        {
        }
        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("SUPPLIER CONTACT", this.ContactName);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
