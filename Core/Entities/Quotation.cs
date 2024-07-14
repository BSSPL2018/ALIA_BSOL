using BSOL.Core.Helpers;
using BSOL.Core.Models.Retail;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class Quotation
    {
        #region Columns
        [JsonConverter(typeof(IdentityEncryptor))]
        [ModelBinder(typeof(IdentityDecryptor))]
        public long Id { get; set; }
        public long ShopId { get; set; }
        public string ShopCode { get; set; }
        public int RefNo { get; set; }
        public string? QuotationType { get; set; }
        public DateTime QuotationDate { get; set; }
        public long CustomerId { get; set; }
        public string CustomerType { get; set; }
        public string CustomerName { get; set; }
        public string ContactName { get; set; }
        public string ContactNo { get; set; }
        public string CustomerRefNo { get; set; }
        public string Currency { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GSTPercent { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; }
        public string ApprovedBy1 { get; set; }
        public DateTime? ApprovedDate1 { get; set; }
        public string ApprovedBy2 { get; set; }
        public DateTime? ApprovedDate2 { get; set; }
        public string ClosedBy { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string ClosedReason { get; set; }
        public long? InvoiceId { get; set; }
        public long? DeliveryId { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string RefNoFormatted { get; set; }
        public string QuotationUnit { get; set; }
        public decimal ActualQty { get; set; }
        public decimal Conversion { get; set; }
        #endregion

        #region Additional Properties
        [NotMapped]
        public string PhoneNo { get; set; }//TODO: use contact no
        [NotMapped]
        public string Email { get; set; }
        [NotMapped]
        public decimal NetAmount { get; set; }
        [NotMapped]
        public decimal TaxableAmount { get; set; }
        [NotMapped]
        public bool AllowNotification { get; set; }
        [NotMapped]
        public int FollowUpCount { get; set; }
        [NotMapped]
        public string ImagePath { get; set; }
        [NotMapped]
        public long ItemId { get; set; }
        [NotMapped]
        public List<SellingUnitModel> SellingUnits { get; set; }
        [NotMapped]
        public decimal ItemRate { get; set; }
        [NotMapped]
        public string Unit { get; set; }
        [NotMapped]
        public string Size { get; set; }
        #endregion
    }
}
