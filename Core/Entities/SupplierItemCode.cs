using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class SupplierCode : BaseObject
    {

        public long SupplierId { get; set; }
        public string SupplierItemCode { get; set; }
        public long ItemId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        [NotMapped]
        public long SKU { get; set; }
        [NotMapped]
        public long? UPC { get; set; }
        [NotMapped]
        public string ItemCode { get; set; }
        [NotMapped]
        public string ItemDescription { get; set; }
        [NotMapped]
        public string Category { get; set; }
        [NotMapped]
        public string SupplierName { get; set; }
        [NotMapped]
        public string SKUFormatted { get; set; }

        protected override async Task OnSave()
        {
            if (await _Webcontext.SupplierItemCodes.AnyAsync(x => x.SupplierId == this.SupplierId && x.ItemId == this.ItemId))
                await Update();
            else
                await Add();
        }
        protected override async Task Validate()
        {
            if (await _Webcontext.SupplierItemCodes.AnyAsync(x => x.SupplierItemCode == this.SupplierItemCode && x.SupplierId == this.SupplierId))
                AddMessage("Suplier item code (" + this.SupplierItemCode + ") already exists");
        }

        protected override async Task Add()
        {
            this.CreatedBy = GetAppUserName();
            this.CreatedOn = DateTime.Now;
            _Webcontext.SupplierItemCodes.Add(this);
            LogAdd("Supplier item code mapping", this.SupplierId, this.ItemId, this.SupplierItemCode);
            await _Webcontext.SaveChangesAsync();
        }


        protected override async Task Update()
        {
            var existing = await _Webcontext.SupplierItemCodes.FirstOrDefaultAsync(x => x.SupplierId == this.SupplierId && x.ItemId == this.ItemId);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            _Webcontext.Entry(existing).Property(x => x.CreatedBy).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.CreatedOn).IsModified = false;
            LogUpdate("Supplier item code mapping", this.SupplierId, this.ItemId, this.SupplierItemCode);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("Supplier item code mapping", this.SupplierId, this.ItemId, this.SupplierItemCode);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
