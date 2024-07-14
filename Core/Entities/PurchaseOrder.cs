using BSOL.Core.Models.Accounts;
using BSOL.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class PurchaseOrder : BaseObject
    {
        #region Columns
        [JsonConverter(typeof(IdentityEncryptor))]
        [ModelBinder(typeof(IdentityDecryptor))]
        public long Id { get; set; }
        public long RefNo { get; set; }
        public long SupplierId { get; set; }
        public string PaymentType { get; set; }
        public DateTime PurchaseOrderDate { get; set; }
        public string Currency { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GSTPercent { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Balance { get; set; }
        public string Remarks { get; set; }
        public string Notes { get; set; }
        public string Mode { get; set; }
        public bool Active { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? VerifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string RefNoFormatted { get; set; }
        public string ClosedBy { get; set; }
        public DateTime? ClosedOn { get; set; }
        public string IncoTerm { get; set; }
        public long? ShopGroupID { get; set; }
        public string QuotationNo { get; set; }
        public DateTime? QuotationDate { get; set; }
        public string ReasonForCancel { get; set; }
        public DateTime? CheckedOn { get; set; }
        public string CheckedBy { get; set; }
        public string OtherRefNo { get; set; }
        #endregion

        #region Additional Properties
        [NotMapped]
        public string Email { get; set; }
        [NotMapped]
        public decimal NetAmount { get; set; }
        [NotMapped]
        public decimal TaxableAmount { get; set; }
        [NotMapped]
        public string SupplierName { get; set; }
        [NotMapped]
        public string ContactName { get; set; }
        [NotMapped]
        public string ContactNo { get; set; }
        [NotMapped]
        public string ShopGroupName { get; set; }
        [NotMapped]
        public string PaymentRequest { get; set; }
        [NotMapped]
        public decimal? RemainingBalance { get; set; }
        [NotMapped]
        public string PaymentRefNo { get; set; }
        #endregion


        protected override async Task Add()
        {
            _Webcontext.PurchaseOrders.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Validate()
        {
            if (await (from sp in _Webcontext.ShipmentPurchaseOrderDetails
                       join pod in _Webcontext.PurchaseOrderDetails on sp.PurchaseOrderDetailId equals pod.Id
                       join s in _Webcontext.Shipments on sp.ShipmentId equals s.Id
                       where pod.PurchaseOrderId == this.Id
                       select pod.PurchaseOrderId).AnyAsync())
                this.AddMessage("Purchase order was referred in shipment");
        }

        protected override async Task Update()
        {
            var existing = await _Webcontext.PurchaseOrders.FindAsync(Id);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("Purchase order", this.RefNoFormatted);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("Purchase order", this.RefNoFormatted);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
