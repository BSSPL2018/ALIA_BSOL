using BSOL.Core.Helpers;
using BSOL.Helpers;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class StockAdjustment : BaseObject
    {
        #region Columns
        public long Id { get; set; }
        public int RefNo { get; set; }
        public long ShopId { get; set; }
        public string ShopCode { get; set; }
        public string Notes { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string RefNoFormatted { get; set; }
        public bool Active { get; set; }
        #endregion

        #region Additional Properties
        [NotMapped]
        public string ShopName { get; set; }
        [NotMapped]
        public string Status { get; set; }
        #endregion

        //protected override async Task Validate()
        //{
        //    var errors = await _Webcontext.ValidateAsync("spBSOL_StockAdjustment", new { Option = "VAL_SAVE", this.Id, this.ItemId });
        //    if (errors.Any())
        //        AddMessage(errors);
        //}
        protected override async Task Add()
        {
            int maxRefNo = (from si in _Webcontext.StockAdjustments
                            where si.CreatedOn.Year == DateTime.Now.Year && si.ShopCode == this.ShopCode
                            select si.RefNo).Max(x => (int?)x) ?? 0;
            this.RefNo = maxRefNo + 1;
            this.Active = true;

            _Webcontext.StockAdjustments.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.StockAdjustments.FindAsync(Id);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            //_Webcontext.Entry(existing).Property(x => x.ItemId).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.VerifiedBy).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.VerifiedDate).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.CreatedBy).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.CreatedOn).IsModified = false;
            LogUpdate("STOCK ADJUSTMENT",this.RefNoFormatted);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Delete()
        {
            var existing = await _Webcontext.StockAdjustments.FindAsync(Id);
            existing.Active = false;
            LogDelete("STOCK ADJUSTMENT", this.RefNoFormatted);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task<bool> OnApprove()
        {
            var existing = await _Webcontext.StockAdjustments.FindAsync(this.Id);

            if (existing.VerifiedDate.HasValue)
                this.AddMessage("Request already verified by " + existing.VerifiedBy);

            if (HasError)
                return false;

            existing.VerifiedBy = base.GetAppUserName();
            existing.VerifiedDate = DateTime.Now;
            Log("STOCK ADJUSTMENT", ActionType.Approve, this.RefNoFormatted);

            await _Webcontext.SaveChangesAsync();

            await UpdateStock("VERIFY");
            return true;
        }
        protected override async Task<bool> OnReject()
        {
            var existing = await _Webcontext.StockAdjustments.FindAsync(this.Id);

            if (!existing.VerifiedDate.HasValue)
                this.AddMessage("Request not yet verified");

            if (HasError)
                return false;

            existing.VerifiedBy = null;
            existing.VerifiedDate = null;
            Log("STOCK ADJUSTMENT", ActionType.Reject, this.RefNoFormatted);

            await _Webcontext.SaveChangesAsync();

            await UpdateStock("REVERT");
            return true;
        }
        private async Task UpdateStock(string Option)
        {
            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_StockAdjustment", new { Option, this.Id, this.ShopId, CreatedBy = this.CreatedBy });
            if (retVal <= 0)
                this.AddMessage(Constants.DBErrorMessage);
        }
    }
}
