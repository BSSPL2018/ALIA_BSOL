using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class BusinessService : BaseObject
    {
        public long ID { get; set; }
        public long BusinessUnitId { get; set; }
        public long? ParentId { get; set; }
        public string ServiceCategory { get; set; }
        public string ServiceName { get; set; }
        public int ServiceNo { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn  { get; set; }
        public long? DebitTo { get; set; }
        public long? CreditTo { get; set; }

        [NotMapped]
        public string BusinessName { get; set; }
        [NotMapped]
        public string DebitAccountName { get; set; }
        [NotMapped]
        public string CreditAccountName { get; set; }
        protected override async Task Validate()
        {
          if (await _Webcontext.BusinessServices.AnyAsync(x => x.BusinessUnitId == this.BusinessUnitId && x.ServiceCategory == this.ServiceCategory && x.ServiceNo == this.ServiceNo && x.ID != this.ID))
                AddMessage(" this Values (" + this.BusinessUnitId + this.ServiceCategory + this.ServiceNo + ") already exists");
        }

        protected override async Task Add()
        {

            _Webcontext.BusinessServices.Add(this);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task Update()
        {
            var existing = await _Webcontext.BusinessServices.FindAsync(ID);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            _Webcontext.Entry(existing).Property(x => x.CreatedBy).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.CreatedOn).IsModified = false;
            LogUpdate("BusinessServices", this.BusinessUnitId);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("BusinessService", this.ID);
            await _Webcontext.SaveChangesAsync();
        }
    }
    }

