using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class InvoiceType : BaseObject
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Type { get; set; }
        public long? IncomeAccountId { get; set; }
        public long? ExpenseAccountId { get; set; }
        public long? DiscountAccountId { get; set; }
        public long? ReturnAccountId { get; set; }

        #region Additional Properties
        [NotMapped]
        public string IncomeAccountName { get; set; }
        [NotMapped]
        public string ExpenseAccountName { get; set; }
        [NotMapped]
        public string DiscountAccountName { get; set; }
        [NotMapped]
        public string ReturnAccountName { get; set; }

        #endregion

        protected override async Task Validate()
        {
            if (await _Webcontext.InvoiceTypes.AnyAsync(x => x.CompanyId == this.CompanyId && x.Type == this.Type && x.Id != this.Id))
                AddMessage("Same Invoice Type (" + this.Type + ") already exists.");
        }

        protected override async Task Add()
        {
            _Webcontext.InvoiceTypes.Add(this);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task Update()
        {
            var existing = await _Webcontext.InvoiceTypes.FindAsync(Id);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("INVOICE TYPES", this.Type);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task CheckDependency()
        {
            
        }

        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("INVOICE TYPES", this.Type);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
