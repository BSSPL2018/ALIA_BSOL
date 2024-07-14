using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class ShipmentExpense : BaseObject
    {
        #region Columns
        public long? Id { get; set; }
        public long ShipmentId { get; set; }
        public long ExpenseCategoriesId { get; set; }
        public string ExpenseName { get; set; }
        public decimal? Amount { get; set; }
        public string Description { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public string Currency { get; set; }
        public decimal? CurrencyRate { get; set; }
        public string EntryBy { get; set; }
        public DateTime? EntryDate { get; set; }
        public long? PaymentRequestId { get; set; }
        public long? GSTSettingId { get; set; }
        public decimal? GSTPercent { get; set; }
        public decimal? GSTAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        #endregion
        #region Additional Properties
        [NotMapped]
        public string ShipmentRefNo { get; set; }
        [NotMapped]
        public string GST { get; set; }


        #endregion
        protected override async Task Validate()
        {
            if (await _Webcontext.ShipmentExpenses.AnyAsync(x => x.ShipmentId == this.ShipmentId && x.Id != this.Id && x.ExpenseCategoriesId == this.ExpenseCategoriesId))
                AddMessage("Same Shipment Expense (" + this.ExpenseName + ")" + " with (" + this.Description + ") already exists");
        }
        protected override async Task Add()
        {
            this.CurrencyRate = await (from c in _Webcontext.Currencies
                                       where c.Currency == this.Currency
                                       select c.ConversionRate).FirstOrDefaultAsync();

            _Webcontext.ShipmentExpenses.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.ShipmentExpenses.FindAsync(Id);
            this.CurrencyRate = await (from c in _Webcontext.Currencies
                                       where c.Currency == this.Currency
                                       select c.ConversionRate).FirstOrDefaultAsync();

            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            _Webcontext.Entry(existing).Property(x => x.ShipmentId).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.EntryBy).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.EntryDate).IsModified = false;
            LogUpdate("SHIPMENT EXPENSES", this.ExpenseName);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task CheckDependency()
        {
            if (await _Webcontext.ItemCostings.AnyAsync(x => x.ShipmentId == this.ShipmentId))
                AddMessage("Item costing added.");
        }
        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("SHIPMENT EXPENSES", this.ExpenseName);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
