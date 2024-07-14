using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class CustomerSellingCode : BaseObject
    {
        public long CustomerId { get; set; }
        [Column("CustomerSellingCode")]
        public string CustomerItemCode { get; set; }
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
        public string CustomerName { get; set; }
        [NotMapped]
        public string SKUFormatted { get; set; }

        protected override async Task OnSave()
        {
            if (await _Webcontext.CustomerSellingCodes.AnyAsync(x => x.CustomerId == this.CustomerId && x.ItemId == this.ItemId))
                await Update();
            else
                await Add();
        }
        protected override async Task Validate()
        {
            if (await _Webcontext.CustomerSellingCodes.AnyAsync(x => x.CustomerItemCode == this.CustomerItemCode && x.CustomerId == this.CustomerId))
                AddMessage("Customer selling item code (" + this.CustomerItemCode + ") already exists");
        }

        protected override async Task Add()
        {
            this.CreatedBy = GetAppUserName();
            this.CreatedOn = DateTime.Now;
            _Webcontext.CustomerSellingCodes.Add(this);
            LogAdd("Customer item code mapping", this.CustomerId, this.ItemId, this.CustomerItemCode);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task Update()
        {
            var existing = await _Webcontext.CustomerSellingCodes.FirstOrDefaultAsync(x => x.CustomerId == this.CustomerId && x.ItemId == this.ItemId);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            _Webcontext.Entry(existing).Property(x => x.CreatedBy).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.CreatedOn).IsModified = false;
            LogUpdate("Customer item code mapping", this.CustomerId, this.ItemId, this.CustomerItemCode);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("Customer item code mapping", this.CustomerId, this.ItemId, this.CustomerItemCode);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
