using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class ItemCategory : BaseObject
    {
        #region Columns
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public long? IncomeAccountId { get; set; }
        public long? ExpenseAccountId { get; set; }
        public long? InventoryAccountId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        #endregion

        #region Additional Properties
        [NotMapped]
        public string IncomeAccountName { get; set; }
        [NotMapped]
        public string ExpenseAccountName { get; set; }
        [NotMapped]
        public string InventoryAccountName { get; set; }
        [NotMapped]
        public string IncomeAccountCode { get; set; }
        [NotMapped]
        public string ExpenseAccountCode { get; set; }
        [NotMapped]
        public string InventoryAccountCode { get; set; }
        #endregion

        protected override async Task Validate()
        {
            if (await _Webcontext.ItemCategories.AnyAsync(x => x.CompanyId == this.CompanyId && x.Category == this.Category && x.Id != this.Id))
                this.AddMessage("Category (" + this.Category + ") already exists");
        }
        protected override async Task Add()
        {
            AccountSetting accountSetting = await _Webcontext.AccountSettings.FindAsync(this.CompanyId);
            this.IncomeAccountId = accountSetting.IncomeAccountId;
            this.ExpenseAccountId = accountSetting.ExpenseAccountId;
            this.InventoryAccountId = accountSetting.InventoryAccountId;
            _Webcontext.ItemCategories.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.ItemCategories.FindAsync(Id);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            _Webcontext.Entry(existing).Property(x => x.IncomeAccountId).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.ExpenseAccountId).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.InventoryAccountId).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.CreatedBy).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.CreatedOn).IsModified = false;
            LogUpdate("ITEM CATEGORY", this.Category);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task CheckDependency()
        {
            if (await _Webcontext.Items.AnyAsync(x => x.ItemCategoryId == this.Id))
                this.AddMessage("Items added for this Category.");
        }
        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("ITEM CATEGORY", this.Category);
            await _Webcontext.SaveChangesAsync();
        }

        public async Task<bool> SaveAccount()
        {
            return await ModifyAsync(async () =>
            {
                var existing = await _Webcontext.ItemCategories.FindAsync(Id);
                existing.IncomeAccountId = this.IncomeAccountId;
                existing.ExpenseAccountId = this.ExpenseAccountId;
                existing.InventoryAccountId = this.InventoryAccountId;
                LogUpdate("ITEM CATEGORY", this.Category);
                await _Webcontext.SaveChangesAsync();
                return true;
            });
        }
    }
}
