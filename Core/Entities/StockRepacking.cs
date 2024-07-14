using BSOL.Core.Helpers;
using BSOL.Helpers;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class StockRepacking : BaseObject
    {
        #region Columns
        public long Id { get; set; }
        public long FromItemId { get; set; }
        public int FromItemQty { get; set; }
        public long ToItemId { get; set; }
        public int ToItemQty { get; set; }
        public string Notes { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        #endregion

        #region Additional Properties
        [NotMapped]
        public string FromSKU { get; set; }
        [NotMapped]
        public string FromItemDescription { get; set; }
        [NotMapped]
        public int FromStock { get; set; }
        [NotMapped]
        public string ToSKU { get; set; }
        [NotMapped]
        public string ToItemDescription { get; set; }
        [NotMapped]
        public int ToStock { get; set; }
        #endregion

        protected override async Task Validate()
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_Repacking", new { Option = "VAL_SAVE", this.Id, this.FromItemId, this.FromItemQty, this.ToItemId });
            if (errors.Any())
                AddMessage(errors);
        }
        protected override async Task Add()
        {
            _Webcontext.StockRepackings.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.StockRepackings.FindAsync(Id);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            _Webcontext.Entry(existing).Property(x => x.FromItemId).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.ToItemId).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.VerifiedBy).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.VerifiedDate).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.CreatedBy).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.CreatedOn).IsModified = false;
            LogUpdate("STOCK REPACKING", this.FromSKU, this.ToSKU);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("STOCK REPACKING", this.FromSKU, this.ToSKU);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task<bool> OnApprove()
        {
            var existing = await _Webcontext.StockRepackings.FindAsync(this.Id);

            if (existing.VerifiedDate.HasValue)
                this.AddMessage("Request already verified by " + existing.VerifiedBy);

            if (HasError)
                return false;

            existing.VerifiedBy =base.GetAppUserName();
            existing.VerifiedDate = DateTime.Now;
            Log("STOCK REPACKING", ActionType.Approve, this.FromSKU, this.ToSKU);

            await _Webcontext.SaveChangesAsync();

            await UpdateStock("VERIFY");
            return true;
        }
        protected override async Task<bool> OnReject()
        {
            var existing = await _Webcontext.StockRepackings.FindAsync(this.Id);

            if (!existing.VerifiedDate.HasValue)
                this.AddMessage("Request not yet verified");

            if (HasError)
                return false;

            existing.VerifiedBy = null;
            existing.VerifiedDate = null;
            Log("STOCK REPACKING", ActionType.Reject, this.FromSKU, this.ToSKU);

            await _Webcontext.SaveChangesAsync();

            await UpdateStock("REVERT");
            return true;
        }
        private async Task UpdateStock(string Option)
        {
            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_Repacking", new { Option, this.Id });
            if (retVal <= 0)
                this.AddMessage(Constants.DBErrorMessage);
        }
    }
}
