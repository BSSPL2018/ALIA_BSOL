using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class AccountDetailType : BaseObject
    {
        #region Columns
        public long Id { get; set; }
        public long AccountTypeId { get; set; }
        public string DetailType { get; set; }
        public long? ParentId { get; set; }
        #endregion

        #region Additional Properties
        [NotMapped]
        public string Type { get; set; }
        #endregion

        protected override async Task Validate()
        {
            if (await _Webcontext.AccountDetailTypes.AnyAsync(x => x.AccountTypeId == this.AccountTypeId && x.DetailType == this.DetailType && (!x.ParentId.HasValue || x.ParentId == this.ParentId) && x.Id != this.Id))
            {
                var type = !this.ParentId.HasValue ? this.Type : await _Webcontext.AccountDetailTypes.Where(x => x.Id == this.ParentId).Select(x => x.DetailType).FirstOrDefaultAsync();
                AddMessage("Same Account Detail Type (" + this.DetailType + ") already exists in " + type);
            }
        }
        protected override async Task Add()
        {
            _Webcontext.AccountDetailTypes.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.AccountDetailTypes.FindAsync(Id);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("ACCOUNT DETAIL TYPE", this.DetailType);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task CheckDependency()
        {
            if (await _Webcontext.AccountDetailTypes.AnyAsync(x => x.ParentId == this.Id))
                AddMessage("Sub Type(s) configured for this Type (" + this.DetailType + ")");
            if (await _Webcontext.Accounts.AnyAsync(x => x.AccountDetailTypeId == this.Id))
                AddMessage("Account(s) configured for this Type (" + this.DetailType + ")");
        }
        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("ACCOUNT DETAIL TYPE", this.DetailType);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
