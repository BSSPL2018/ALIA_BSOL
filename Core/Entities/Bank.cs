using Microsoft.EntityFrameworkCore;

namespace BSOL.Core.Entities
{
    public class Bank:BaseObject
    {
        #region Columns
        public int ID { get; set; }
        public long CompanyID { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        #endregion

        protected override async Task Add()
        {
            _Webcontext.Banks.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Validate()
        {
            if (await _Webcontext.Banks.AnyAsync(x => x.CompanyID == this.CompanyID && x.BankName == this.BankName && x.ID != this.ID))
                AddMessage("Same Bank Name already exists");
            if (await _Webcontext.Banks.AnyAsync(x => x.CompanyID == this.CompanyID && x.BankCode == this.BankCode && x.ID != this.ID))
                AddMessage("Same Bank Code already exists");
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.Banks.FindAsync(ID);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("Banks", this.BankName, this.BankCode);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task CheckDependency()
        {
            if (await _Webcontext.CompanyAccounts.AnyAsync(x => x.BankID == this.ID))
                AddMessage("Company Accounts added.");
        }
        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("Banks", this.BankName, this.BankCode);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
