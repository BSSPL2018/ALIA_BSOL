using BSOL.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class Invoice
    {
        #region Columns

        [JsonConverter(typeof(IdentityEncryptor))]
        [ModelBinder(typeof(IdentityDecryptor))]
        public long Id { get; set; }
        public long ShopId { get; set; }
        public string ShopCode { get; set; }
        public int RefNo { get; set; }
        public long InvoiceTypeId { get; set; }
        public string PaymentType { get; set; }
        public DateTime InvoiceDate { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string ContactNo { get; set; }
        public string PassportNo { get; set; }
        public long? ShopRoomId { get; set; }
        public string OrderNo { get; set; }
        public string Currency { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public long? DiscountAccountId { get; set; }
        public decimal GSTPercent { get; set; }
        public decimal GSTAmount { get; set; }
        public long? GSTAccountId { get; set; }
        public decimal ServiceChargePercent { get; set; }
        public decimal ServiceChargeAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Balance { get; set; }
        public string BilledBy { get; set; }
        public string Notes { get; set; }
        public string ApprovedBy1 { get; set; }
        public DateTime? ApprovedDate1 { get; set; }
        public string ApprovedBy2 { get; set; }
        public DateTime? ApprovedDate2 { get; set; }
        public decimal? OpeningBalance { get; set; }
        public DateTime? OBDate { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string RefNoFormatted { get; set; }
        public string LinkId { get; set; }
        public decimal OtherCharges { get; set; }
        public decimal TenderedAmount { get; set; }
        public decimal BalanceGiven { get; set; }
        public bool IsDeliverable { get; set; }
        public decimal? TotalProfit { get; set; }
        public long? CounterId { get; set; }
        #endregion

        #region Additional Properties
        [NotMapped]
        public string Type { get; set; }
        [NotMapped]
        public string PhoneNo { get; set; }//TODO: use contact no
        [NotMapped]
        public string Email { get; set; }
        [NotMapped]
        public decimal GrossAmount { get; set; }
        [NotMapped]
        public decimal NetAmount { get; set; }
        [NotMapped]
        public decimal TaxableAmount { get; set; }
        [NotMapped]
        public bool AllowNotification { get; set; }
        [NotMapped]
        public string Item { get; set; }
        [NotMapped]
        public decimal PaidAmount { get; set; }
        [NotMapped]
        public string ChequeNo { get; set; }
        [NotMapped]
        public string BankName { get; set; }
        [NotMapped]
        public DateTime? ChequeDate { get; set; }
        [NotMapped]
        public long ItemId { get; set; }
        [NotMapped]
        public string DeliveryNo { get; set; }
		[NotMapped]
		public decimal? ProfitPercent { get; set; }
		[NotMapped]
		public decimal? ProfitAmount { get; set; }
		#endregion
	}
}
