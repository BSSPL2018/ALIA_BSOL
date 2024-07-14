using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class Supplier : BaseObject
    {
        #region Columns
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public string RegNo { get; set; }
        public string GSTIN { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Notes { get; set; }
        public decimal? OpeningBalance { get; set; }
        public DateTime? OBDate { get; set; }
        public string? EntryBy { get; set; }
        public DateTime? EntryDate { get; set; }
        public decimal? GST { get; set; }
        public bool Active { get; set; }
        public string Type { get; set; }
        public long? ZipCode { get; set; }
        public string Currency { get; set; }
        public int? DuePeriodDays { get; set; }
        public string VendorGroup { get; set; }
        public string ChequeAccountName { get; set; }
        #endregion

        [NotMapped]
        public List<long> ServiceIds { get; set; }
        [NotMapped]
        public string ServiceIdlsts { get; set; }
        [NotMapped]
        public string BankName { get; set; }
        [NotMapped]
        public string AccountName { get; set; }
        [NotMapped]
        public long? AccountNo { get; set; }
        [NotMapped]
        public string Branch { get; set; }
        [NotMapped]
        public string SwiftCode { get; set; }

        protected override async Task Validate()
        {
            if (await _Webcontext.Suppliers.AnyAsync(x => x.CompanyId == this.CompanyId && x.SupplierCode == this.SupplierCode && x.Id != this.Id && x.Active))
                AddMessage("Same Supplier Code (" + this.SupplierCode + ") already exists");

            if (await _Webcontext.Suppliers.AnyAsync(x => x.CompanyId == this.CompanyId && x.SupplierName == this.SupplierName && x.Id != this.Id && x.Active))
                AddMessage("Same Supplier Name (" + this.SupplierName + ") already exists");
        }
        protected override async Task Add()
        {
            this.Active = true;
            _Webcontext.Suppliers.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.Suppliers.FindAsync(Id);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            _Webcontext.Entry(existing).Property(x => x.EntryBy).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.EntryDate).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.Active).IsModified = false;
            LogUpdate("SUPPLIER", this.SupplierCode);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task CheckDependency()
        {
            //if (await _Webcontext.SupplierPayments.AnyAsync(x => x.SupplierId == this.Id && x.Active))
            //    AddMessage("Supplier Payments added for " + this.SupplierName);
        }
        protected override async Task Delete()
        {
            var existing = await _Webcontext.Suppliers.FindAsync(Id);
            existing.Active = false;
            LogDelete("SUPPLIER", this.SupplierCode);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task<bool> OnReject()
        {
            var existing = await _Webcontext.Suppliers.FindAsync(Id);
            existing.Active = true;
            LogDelete("SUPPLIER", this.SupplierCode);
            await _Webcontext.SaveChangesAsync();
            return true;
        }
    }
}
