using BSOL.Helpers;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class Shop : BaseObject
    {
        #region Columns
        public long Id { get; set; }
        public long ShopGroupID { get; set; }
        public long CompanyId { get; set; }
        public string ShopName { get; set; }
        public string ShortName { get; set; }
        public string ShopCode { get; set; }
        public decimal? GST { get; set; }
        public long? GSTSettingId { get; set; }
        public decimal ServiceCharge { get; set; }
        public string Created { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string Verified { get; set; }
        public DateTime? VerifiedOn { get; set; }
        public int? OpeningStock { get; set; }
        public bool IsHead { get; set; }
        public bool Active { get; set; }
        public decimal? MovingAmount { get; set; }
        public decimal? FloatAmountMVR { get; set; }
        public decimal? OpeningFloatMVR { get; set; }
        public decimal? OpeningFloatUSD { get; set; }
        public int? NoOfCounter { get; set; }
        #endregion

        #region Additional Properties
        [NotMapped]
        public string Option { get; set; }
        [NotMapped]
        public int Status { get; set; }
        [NotMapped]
        public int Stock { get; set; }
        [NotMapped]
        public string GSTSettingName { get; set; }
        [NotMapped]
        public long? GSTInAccountId { get; set; }
        [NotMapped]
        public long? GSTOutAccountId { get; set; }
        [NotMapped]
        public string Name { get; set; }
        #endregion

        protected override async Task Add()
        {
            this.Active = true;
            this.GST = 0;
            _Webcontext.Shops.Add(this);
            //LogAdd("Item");
            await _Webcontext.SaveChangesAsync();

            await AddShop();
        }

        protected override async Task Validate()
        {
            if (await _Webcontext.Shops.AnyAsync(x => x.ShopName == this.ShopName && x.Id != this.Id))
                AddMessage("Same shop (" + this.ShopName + ") already exists");
            else if (await _Webcontext.Shops.AnyAsync(x => x.ShopCode == this.ShopCode && x.Id != this.Id))
                AddMessage("Same shop code (" + this.ShopCode + ") already exists");
            else if (await _Webcontext.Shops.AnyAsync(x => x.Id == this.Id && x.Active == false))
                AddMessage("Shop (" + this.ShopCode + ") deactivated");
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.Shops.FindAsync(Id);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            _Webcontext.Entry(existing).Property(x => x.IsHead).IsModified = false;
            LogUpdate("Shops", this.ShopCode, this.ShopName);
            await _Webcontext.SaveChangesAsync();

            await AddShop();
        }
        protected override async Task CheckDependency()
        {
        }

        protected override async Task Delete()
        {
            var existing = await _Webcontext.Shops.FindAsync(Id);
            existing.Active = false;
            LogUpdate("Shops", this.ShopCode, this.ShopName);
            await _Webcontext.SaveChangesAsync();

            await DeleteShop();
        }

        protected override async Task<bool> OnApprove()
        {
            var existing = await _Webcontext.Shops.FindAsync(Id);

            if (existing.VerifiedOn.HasValue)
                this.AddMessage("Shop already verified by " + existing.Verified);

            if (HasError)
                return false;

            existing.Verified = base.GetAppUserName();
            existing.VerifiedOn = DateTime.Now;

            LogUpdate("SHOPS", existing.ShopCode + " - " + existing.ShopName);
            await _Webcontext.SaveChangesAsync();
            return true;
        }

        protected override async Task<bool> OnReject()
        {
            var existing = await _Webcontext.Shops.FindAsync(Id);

            if (!existing.VerifiedOn.HasValue)
                this.AddMessage("Shop not yet verified");

            if (HasError)
                return false;

            existing.Verified = null;
            existing.VerifiedOn = null;

            LogUpdate("SHOPS", existing.ShopCode + " - " + existing.ShopName);
            await _Webcontext.SaveChangesAsync();
            return true;
        }
        private async Task AddShop()
        {
            await _context.ExecuteNonQueryAsync("spPOS_OutletSetting", new
            {
                Option = "WEB_INSERT",
                Outlet = this.ShopName,
                Counter = this.NoOfCounter,
                MovingAmnt = this.MovingAmount,
                FloatAmnt = this.FloatAmountMVR > 0 ? this.FloatAmountMVR : 0,
                MaxAmnt = this.OpeningFloatMVR > 0 ? this.OpeningFloatMVR : 0,
                MaxAmntUSD = this.OpeningFloatUSD > 0 ? this.OpeningFloatUSD : 0,
                WebID = this.Id
            });
        }
        private async Task DeleteShop()
        {
            await _context.ExecuteNonQueryAsync("spPOS_OutletSetting", new
            {
                Option = "WEB_DELETE",
                Outlet = this.ShopName,
                Counter = this.NoOfCounter,
                MovingAmnt = this.MovingAmount,
                FloatAmnt = this.FloatAmountMVR,
                MaxAmnt = this.OpeningFloatMVR,
                MaxAmntUSD = this.OpeningFloatUSD,
                WebID = this.Id
            });
        }
    }
}

