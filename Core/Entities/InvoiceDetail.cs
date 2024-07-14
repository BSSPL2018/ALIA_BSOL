using BSOL.Core.Helpers;
using BSOL.Core.Models.Retail;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BSOL.Core.Entities
{
    public class InvoiceDetail
    {
        #region Columns
        public long Id { get; set; }
        public long? InvoiceId { get; set; }
        public long ItemId { get; set; }
        public long? SerialItemId { get; set; }
        public string Description { get; set; }
        public decimal PurchasedRate { get; set; }
        public decimal BaseRate { get; set; }
        public decimal Rate { get; set; }
        public decimal Qty { get; set; }
        public decimal TotalPrice { get; set; }
        public long? IncomeAccountId { get; set; }
        public long? QuotationDetailId { get; set; }
        public decimal ReturnedQty { get; set; }
        public long? SalesItemReturnId { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public long? DeliveryId { get; set; }
        public long? ProformaDetailId { get; set; }
        public string SellingUnit { get; set; }
        public decimal ActualQty { get; set; }
        public decimal Conversion { get; set; }
        public decimal? MinSellingRate { get; set; }
        public decimal? ProfitPercent { get; set; }
        public decimal? ProfitAmount { get; set; }
        #endregion

        #region Additional Properties
        [NotMapped]
        public int SNO { get; set; }
        [NotMapped]
        public long SKU { get; set; }
        [NotMapped]
        public long? UPC { get; set; }
        [NotMapped]
        public string ItemCode { get; set; }
        [NotMapped]
        public string? SearchText { get; set; }
        [NotMapped]
        public string ItemDescription { get; set; }
        [NotMapped]
        public string SerialNo { get; set; }
        [NotMapped]
        public decimal OldQty { get; set; }
        [NotMapped]
        public decimal? ShopStock { get; set; }
        [NotMapped]
        public string IncomeAccountName { get; set; }
        [NotMapped]
        public string IncomeAccountCode { get; set; }
        [NotMapped]
        public bool IsInventory { get; set; }
        [NotMapped]
        public bool IsSerialized { get; set; }
        [NotMapped]
        public bool IsPerishable { get; set; }
        [NotMapped]
        public bool GSTApplicable { get; set; }
        [NotMapped]
        public decimal TotalAmount { get; set; }
        [NotMapped]
        public bool Selected { get; set; }
        [NotMapped]
        public string ImagePath { get; set; }
        [NotMapped]
        public string CustomerCode { get; set; }
        [NotMapped]
        public string DiscountType { get; set; }
        [NotMapped]
        public decimal? Discount { get; set; }
        [NotMapped]
        public bool IsSerializedNew { get; set; }
        [NotMapped]
        public decimal MasterStock { get; set; }
        [NotMapped]
        public decimal? PaidAmount { get; set; }
        [NotMapped]
        public List<SellingUnitModel> SellingUnits { get; set; }
        [NotMapped]
        public string Unit { get; set; }
        [NotMapped]
        public decimal? TenderedAmount { get; set; }
        [NotMapped]
        public decimal? BalanceGiven { get; set; }
        [NotMapped]
        public decimal? OtherCharges { get; set; }
        [NotMapped]
        public string PaymentType { get; set; }
        [NotMapped]
        public string Size { get; set; }
        [NotMapped]
        public bool IsDeliverable { get; set; }
        [NotMapped]
        public decimal? TotalProfit { get; set; }
        [NotMapped]
        public long? PerishableItemCount { get; set; }
        #endregion
    }
}
