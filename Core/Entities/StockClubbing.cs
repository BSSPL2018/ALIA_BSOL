using BSOL.Core.Helpers;
using BSOL.Helpers;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class StockClubbing : BaseObject
    {
        #region Columns
        public long Id { get; set; }
        public long ItemId { get; set; }
        public decimal Qty { get; set; }
        public string Notes { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        #endregion

        #region Additional Properties
        [NotMapped]
        public long SKU { get; set; }
        [NotMapped]
        public string ItemCode { get; set; }
        [NotMapped]
        public string ItemDescription { get; set; }
        #endregion

        protected override async Task Add()
        {
            _Webcontext.StockClubbings.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.StockClubbings.FindAsync(Id);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            _Webcontext.Entry(existing).Property(x => x.ItemId).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.VerifiedBy).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.VerifiedDate).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.CreatedBy).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.CreatedOn).IsModified = false;
            LogUpdate("STOCK CLUBBING", this.SKU);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("STOCK CLUBBING", this.SKU);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task<bool> OnApprove()
        {
            var existing = await _Webcontext.StockClubbings.FindAsync(this.Id);

            if (existing.VerifiedDate.HasValue)
                this.AddMessage("Request already verified by " + existing.VerifiedBy);

            if (HasError)
                return false;

            existing.VerifiedBy = base.GetAppUserName();
            existing.VerifiedDate = DateTime.Now;
            Log("STOCK CLUBBING", ActionType.Approve, this.SKU);

            await _Webcontext.SaveChangesAsync();

            await UpdateStock("VERIFY");
            return true;
        }
        protected override async Task<bool> OnReject()
        {
            var existing = await _Webcontext.StockClubbings.FindAsync(this.Id);

            if (!existing.VerifiedDate.HasValue)
                this.AddMessage("Request not yet verified");

            if (HasError)
                return false;

            existing.VerifiedBy = null;
            existing.VerifiedDate = null;
            Log("STOCK CLUBBING", ActionType.Reject, this.SKU);

            await _Webcontext.SaveChangesAsync();

            await UpdateStock("REVERT");
            return true;
        }
        private async Task UpdateStock(string Option)
        {
            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_Clubbing", new { Option, this.Id });
            if (retVal <= 0)
                this.AddMessage(Constants.DBErrorMessage);
        }
    }
}
