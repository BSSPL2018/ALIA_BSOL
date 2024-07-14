using BSOL.Core.Helpers;
using BSOL.Core.Models.Retail;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BSOL.Core.Entities
{
    public class PurchaseOrderDetail
    {
        #region Columns

        public long Id { get; set; }
        public long PurchaseOrderId { get; set; }
        public long ItemId { get; set; }
        public string Description { get; set; }
        public decimal BaseRate { get; set; }
        public decimal Rate { get; set; }
        public decimal RequestedQty { get; set; }
        public decimal ConfirmedQty { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal ReceivedQty { get; set; }
        public string ReceivedBy { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? ReceivedOn { get; set; }
        public decimal OldQty { get; set; }
        public string Remarks { get; set; }
        public decimal ReturnedQty { get; set; }
        public decimal ActualRequestedQty { get; set; }
		public decimal ActualConfirmedQty { get; set; }
		public decimal ActualReceivedQty { get; set; }
		public decimal ActualReturnedQty { get; set; }
		public string PurchaseUnit { get; set; }
		public decimal Conversion { get; set; }
        public decimal? GSTAmount { get; set; }
        public decimal? GSTPercent { get; set; }
        public decimal? DiscountAmount { get; set; }

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
        public string SupplierCode { get; set; }
        [NotMapped]
        public string ItemDescription { get; set; }
        [NotMapped]
        public string ItemCategory { get; set; }
        [NotMapped]
        public string ImagePath { get; set; }
        [NotMapped]
        public bool GSTApplicable { get; set; }
        [NotMapped]
        public bool IsInventory { get; set; }
        [NotMapped]
        public bool Selected { get; set; }
		[NotMapped]
		public List<SellingUnitModel> PurchaseUnits { get; set; }
		[NotMapped]
		public string Unit { get; set; }
		[NotMapped]
		public string Size { get; set; }
		#endregion
	}
}
