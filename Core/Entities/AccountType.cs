using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class AccountType : BaseObject
    {
        #region Columns
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        #endregion

        protected override async Task Validate()
        {
            if (await _Webcontext.AccountTypes.AnyAsync(x => x.CompanyId == this.CompanyId && x.Type == this.Type && x.Id != this.Id))
                AddMessage("Same Account Type (" + this.Type + ") already exists");
        }
        protected override async Task Add()
        {
            _Webcontext.AccountTypes.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.AccountTypes.FindAsync(Id);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);

            LogUpdate("ACCOUNT TYPE", this.Type);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task CheckDependency()
        {
            if (await _Webcontext.AccountDetailTypes.AnyAsync(x => x.AccountTypeId == this.Id))
                AddMessage("Account Detail Type entered for this Type");
        }
        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("ACCOUNT TYPE", this.Type);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
