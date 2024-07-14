using BSOL.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace BSOL.Core.Entities
{
    public class Item : BaseObject
    {
        #region Columns
        public long Id { get; set; }
        public long ItemCategoryId { get; set; }
        public long? SupplierId { get; set; }
        public long? SalesCategoryId { get; set; }
        public string ItemType { get; set; }
        public long SKU { get; set; }
        public long? UPC { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public string Size { get; set; }
        public bool IsSalable { get; set; }
        public bool IsInventory { get; set; }
        public bool IsPerishable { get; set; }
        public bool IsSerialized { get; set; }
        public long? HSNId { get; set; }
        public decimal PurchasedRate { get; set; }
        public decimal SellingRate { get; set; }
        public decimal Stock { get; set; }
        public decimal OpeningStock { get; set; }
        public decimal OpeningCostPrice { get; set; }
        public DateTime? OpeningStockDate { get; set; }
        public decimal LowStockAlertQty { get; set; }
        public bool GSTApplicable { get; set; }
        public string ImagePath { get; set; }
        public string Notes { get; set; }
        public bool Active { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? VerifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SKUFormatted { get; set; }

        public string SellingUnit { get; set; }
        public string VendorName { get; set; }
        public string OriginName { get; set; }
        public string Brand { get; set; }

        public string ModelCode { get; set; }
        public string Color { get; set; }
        public decimal Surcharge { get; set; }

        public decimal? Height { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? CBM { get; set; }
        public string GenericName { get; set; }
        public string Import { get; set; }
        #endregion

        #region Additional Properties
        [NotMapped]
        public string Category { get; set; }
        [NotMapped]
        public string SalesCategory { get; set; }
        [NotMapped]
        public string HSNCode { get; set; }
        [NotMapped]
        public string SupplierCode { get; set; }

        [NotMapped]
        public bool Selected { get; set; }
        [NotMapped]
        public string ItemCategory { get; set; }
        [NotMapped]
        public string ItemDescription { get; set; }
        [NotMapped]
        public string Option { get; set; }
        [NotMapped]
        public int Status { get; set; }

        [NotMapped]
        public decimal MasterStock { get; set; }
        [NotMapped]
        public decimal Qty { get; set; }
        [NotMapped]
        public decimal Rate { get; set; }
        [NotMapped]
        public decimal TotalPrice { get; set; }
        [NotMapped]
        public string AccountName { get; set; }
        [NotMapped]
        public decimal AvailableStock { get; set; }
        [NotMapped]
        public string Mode { get; set; }
        [NotMapped]
        public string SupplierName { get; set; }
        [NotMapped]
        public List<SellingItemUnit> AdjustmentUnits { get; set; }
        [NotMapped]
        public List<SellingItemUnit> SellingItemUnits { get; set; }
        [NotMapped]
        public decimal? ActualQty { get; set; }
        [NotMapped]
        public decimal? Conversion { get; set; }
        #endregion

        protected override async Task Validate()
        {
            if (this.Id > 0)
            {
                var existingItem = await _Webcontext.Items.AsNoTracking().FirstOrDefaultAsync(x => x.Id == this.Id);
                if (!existingItem.Active)
                    AddMessage("Item is In-Active.");

                if (existingItem.VerifiedOn.HasValue)
                    AddMessage("Item was verified.");

                if (existingItem.OpeningStock != this.OpeningStock && existingItem.Stock != existingItem.OpeningStock)
                    AddMessage("Cannot change Opening Stock. Some transactions were made for this Item.");
            }

            if (await _Webcontext.Items.AnyAsync(x => x.SKU == this.SKU && x.Id != this.Id))
                AddMessage("SKU (" + this.SKU + ") already exists");

            //if (await _Webcontext.Items.AnyAsync(x => x.UPC == this.UPC && x.Id != this.Id))
            //    AddMessage("UPC (" + this.UPC + ") already exists");

            if (await _Webcontext.Items.AnyAsync(x => x.ItemCode == this.ItemCode && x.ItemCategoryId == this.ItemCategoryId && this.ItemType != "Unit" && x.Id != this.Id))
                AddMessage("Item code (" + this.ItemCode + ") already exists");

            if (await _Webcontext.Items.AnyAsync(x => x.ItemCode == this.ItemCode && x.ModelCode == this.ModelCode && x.ItemCategoryId == this.ItemCategoryId && this.ItemType == "Unit" && x.Id != this.Id))
                AddMessage("Item code (" + this.ItemCode + ") and (" + this.ModelCode + ") already exists");
        }

        protected override async Task Add()
        {
            this.Stock = this.IsInventory ? this.OpeningStock : 0;
            this.OpeningCostPrice = this.OpeningStock > 0 && this.PurchasedRate > 0 ? this.PurchasedRate : 0;

            //long lsku = await _Webcontext.Items.Where(x => x.SKU.ToString().StartsWith("30")).OrderByDescending(x => x.SKU).Select(x => x.SKU).FirstOrDefaultAsync();
            this.SKU = this.SKU > 0 ? this.SKU : 0;

            this.Active = true;
            _Webcontext.Items.Add(this);
            await _Webcontext.SaveChangesAsync();


            ///* Assign item to all the Shops */
            await _Webcontext.ExecuteNonQueryAsync("spBSOL_Items", new { Option = "ASSIGN_ITEM_TO_ALL_SHOPS", ItemId = this.Id });

            /* Add opening cost to RateHistory */
            await _Webcontext.ExecuteNonQueryAsync("spBSOL_AddItemRateHistory", new { ItemId = this.Id });
        }

        protected override async Task Update()
        {
            this.OpeningCostPrice = this.OpeningCostPrice == 0 && this.OpeningStock > 0 && this.PurchasedRate > 0 ? this.PurchasedRate : 0;

            var existing = await _Webcontext.Items.FindAsync(Id);
            var isOpeningStockChanged = existing.OpeningStock != this.OpeningStock;
            var isOpeningCostChanged = existing.OpeningCostPrice != this.OpeningCostPrice;
            if (isOpeningStockChanged)
                this.Stock = this.OpeningStock;
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            if (!isOpeningStockChanged)
                _Webcontext.Entry(existing).Property(x => x.Stock).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.Active).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.VerifiedBy).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.VerifiedOn).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.CreatedBy).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.CreatedOn).IsModified = false;
            LogUpdate("ITEMS", this.SKU);

            if (isOpeningStockChanged)
            {
                var headShopId = (long?)await _Webcontext.Shops.AsNoTracking().Where(x => x.IsHead).Select(x => x.Id).FirstOrDefaultAsync();
                if (headShopId.HasValue)
                {
                    var shopItem = await _Webcontext.ShopItems.FirstOrDefaultAsync(x => x.ShopId == headShopId.Value && x.ItemId == this.Id);
                    if (shopItem != null)
                        shopItem.Stock = Convert.ToDecimal(this.Stock);
                    shopItem.OpeningStock = Convert.ToDecimal(this.Stock);
                }
            }
            await _Webcontext.SaveChangesAsync();

            long sid = await (from su in _Webcontext.SellingItemUnits
                              join um in _Webcontext.UnitOfMeasures on su.SellingUnitId equals um.ID
                              where um.UOM == this.Unit && su.ItemId == this.Id
                              select su.Id).FirstOrDefaultAsync();

            _Webcontext.SellingItemUnits.RemoveRange(_Webcontext.SellingItemUnits.Where(x => x.Id == sid));
            await _Webcontext.SaveChangesAsync();

            if (isOpeningCostChanged)
                await _Webcontext.ExecuteNonQueryAsync("spBSOL_AddItemRateHistory", new { ItemId = this.Id });
        }

        protected override async Task CheckDependency()
        {
            if (await _Webcontext.Items.AnyAsync(x => x.Id == this.Id && x.VerifiedOn != null))
                this.AddMessage("Item was verified");

            //if (await _Webcontext.ShopItems.AnyAsync(x => x.ItemId == this.Id))
            //    this.AddMessage("Item assigned to shops");

            if (await _Webcontext.PurchaseOrderDetails.AnyAsync(x => x.ItemId == this.Id))
                this.AddMessage("Purchase Order entered for this Item");

            if (await _Webcontext.InvoiceDetails.AnyAsync(x => x.ItemId == this.Id))
                this.AddMessage("Invoice raised for this Item");

            if (await _Webcontext.SerialedItems.AnyAsync(x => x.ItemId == this.Id))
                this.AddMessage("Serialed Items/Perishable Items entered for this Item");
        }

        protected override async Task Delete()
        {
            _Webcontext.ShopItems.RemoveRange(_Webcontext.ShopItems.Where(x => x.ItemId == this.Id));
            await _Webcontext.SaveChangesAsync();

            var existing = await _Webcontext.Items.FindAsync(Id);
            //existing.Active = false;
            _Webcontext.Items.Remove(existing);
            await _Webcontext.SaveChangesAsync();

            Log("ITEMS", ActionType.Delete, this.SKU);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task<bool> OnApprove()
        {
            var existing = await _Webcontext.Items.FindAsync(this.Id);

            if (existing.VerifiedOn.HasValue)
                this.AddMessage("Item already verified by " + existing.VerifiedBy);
            //if (existing.CreatedBy == GetAppUserName() && !GetAppUserName().Contains("LS10"))//Allowed for Haider
            //    this.AddMessage("You cannot approve your own request");

            if (HasError)
                return false;

            existing.VerifiedBy = base.GetAppUserName();
            existing.VerifiedOn = DateTime.Now;

            Log("ITEMS", ActionType.Approve, this.SKU);
            await _Webcontext.SaveChangesAsync();
            return true;
        }

        protected override async Task<bool> OnReject()
        {
            var existing = await _Webcontext.Items.FindAsync(this.Id);

            if (!existing.VerifiedOn.HasValue)
                this.AddMessage("Item not yet verified");
            //if (existing.CreatedBy == GetAppUserName() && !GetAppUserName().Contains("LS10"))//Allowed for Haider
            //    this.AddMessage("You cannot reject your own request");

            if (HasError)
                return false;

            existing.VerifiedBy = null;
            existing.VerifiedOn = null;

            Log("ITEMS", ActionType.Reject, this.SKU);
            await _Webcontext.SaveChangesAsync();
            return true;
        }
    }
}
