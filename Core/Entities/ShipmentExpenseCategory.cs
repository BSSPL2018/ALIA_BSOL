using DocumentFormat.OpenXml.Office2010.Excel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class ShipmentExpenseCategory :BaseObject
    {
        public long Id { get; set; }
        public string ShipmentType { get; set; }
        public long ItemCategoryId { get; set; }
        public string ExpenseCategory { get; set; }
        public string ExpenseCategoryDetails { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public string MaxAmount { get; set; }

        public bool  Active { get; set; }

        [NotMapped]
        public string Category { get; set; }

        
 
        protected override async Task Add()
        {
            this.Active=true;
            _Webcontext.ShipmentExpenseCategories.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
         protected override async Task Update()
        {
            var existing = await _Webcontext.ShipmentExpenseCategories.FindAsync(Id);
  
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
             LogUpdate("ShipmentExpenseCategory",this.Id,this.ItemCategoryId);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task<bool> Delete()
        {
            var existing = await _Webcontext.ShipmentExpenseCategories.FindAsync(Id);
            existing.Active=false;
            LogDelete("ShipmentExpenseCategory", this.Id, this.ItemCategoryId);
            await _Webcontext.SaveChangesAsync();
            return true;
        }
        protected override async Task<bool> OnReject()
        {
            var existing = await _Webcontext.ShipmentExpenseCategories.FindAsync(Id);
            existing.Active=true;
            Log("ShipmentExpenseCategory", ActionType.Reject);
            await _Webcontext.SaveChangesAsync();
            return true;
        }
    }
}
