using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class Account : BaseObject
    {
        #region Columns
        public long Id { get; set; }
        public long AccountDetailTypeId { get; set; }
        public string AccountName { get; set; }
        public string Code { get; set; }
        public string Currency { get; set; }
        public string ActualCurrency { get; set; }
        public decimal OpeningBalance { get; set; }
        public DateTime? BalanceOn { get; set; }
        #endregion

        #region Additional Properties
        [NotMapped]
        public string Type { get; set; }
        [NotMapped]
        public string SubType { get; set; }
        [NotMapped]
        public string DetailType { get; set; }
        [NotMapped]
        public long AccountTypeId { get; set; }

        #endregion

        protected override async Task Validate()
        {
            if (await (from a in _Webcontext.Accounts
                       join dt in _Webcontext.AccountDetailTypes on a.AccountDetailTypeId equals dt.Id
                       join t in _Webcontext.AccountTypes on dt.AccountTypeId equals t.Id

                       where t.CompanyId == GetCompanyId() && a.AccountName == this.AccountName && a.Id != this.Id
                       select a.Id).AnyAsync())
                AddMessage("Same Account Name (" + this.AccountName + ") already exists");

            if (await (from a in _Webcontext.Accounts
                       join dt in _Webcontext.AccountDetailTypes on a.AccountDetailTypeId equals dt.Id
                       join t in _Webcontext.AccountTypes on dt.AccountTypeId equals t.Id
                       where t.CompanyId == GetCompanyId() && a.Code == this.Code && a.Id != this.Id
                       select a.Id).AnyAsync())
                AddMessage("Same Code (" + this.Code.ToString() + ") already exists");
        }
        protected override async Task Add()
        {
            _Webcontext.Accounts.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.Accounts.FindAsync(Id);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("ACCOUNT", this.AccountName);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task CheckDependency()
        {
            if (await _Webcontext.AccountSettings.AnyAsync(x => x.ReceivableUSDAccountId == this.Id
                                                     || x.ReceivableMVRAccountId == this.Id
                                                     || x.PayableUSDAccountId == this.Id
                                                     || x.PayableMVRAccountId == this.Id
                                                     || x.GSTPayableAccountId == this.Id
                                                     || x.IncomeAccountId == this.Id
                                                     || x.ExpenseAccountId == this.Id
                                                     || x.InventoryAccountId == this.Id))
                AddMessage("Configured in Account Settings");
        }
        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("ACCOUNT", this.AccountName);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
