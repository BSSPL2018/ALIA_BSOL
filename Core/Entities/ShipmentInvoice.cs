using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class ShipmentInvoice : BaseObject
    {
        #region Columns
        public long Id { get; set; }
        public long RefNo { get; set; }
        public long ShipmentId { get; set; }
        public long PurchaseOrderId { get; set; }
        public string BLNo { get; set; }
        public string SupplierInvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public double? InvoiceCBM { get; set; }
        public double? BLCBM { get; set; }
        public decimal CostOfGoods { get; set; }
        public decimal Freight { get; set; }
        public decimal Insurance { get; set; }
        public decimal CustomDutyFee { get; set; }
        public decimal TotalAmount { get; set; }
        public string SerialedItemStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string RefNoFormatted { get; set; }
        #endregion

        #region Additional Properties
        [NotMapped]
        public string ShipmentRefNo { get; set; }
        [NotMapped]
        public string PurchaseOrderRefNo { get; set; }
        #endregion

        protected override async Task Validate()
        {
            if (await _Webcontext.ShipmentInvoices.AnyAsync(x => x.ShipmentId == this.ShipmentId && x.PurchaseOrderId == this.PurchaseOrderId && x.Id != this.Id))
                AddMessage("Same Purchase Order (" + this.PurchaseOrderRefNo + ") already Invoiced");
        }
        protected override async Task Add()
        {
            long lastRefNo = await (from si in _Webcontext.ShipmentInvoices
                                    join sh in _Webcontext.Shipments on si.ShipmentId equals sh.Id
                                    where sh.CompanyId == this.GetCompanyId()
                                    select si.RefNo).MaxAsync(x => (long?)x) ?? 0;
            this.RefNo = ++lastRefNo;
            this.SerialedItemStatus = (await (from spod in _Webcontext.ShipmentPurchaseOrderDetails
                                              join pod in _Webcontext.PurchaseOrderDetails on spod.PurchaseOrderDetailId equals pod.Id
                                              join im in _Webcontext.Items on pod.ItemId equals im.Id
                                              where spod.ShipmentId == this.ShipmentId && im.IsSerialized
                                              select im.Id).AnyAsync()) ? "Pending" : "NA";
            _Webcontext.ShipmentInvoices.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.ShipmentInvoices.FindAsync(Id);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            _Webcontext.Entry(existing).Property(x => x.ShipmentId).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.PurchaseOrderId).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.CreatedBy).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.CreatedOn).IsModified = false;
            LogUpdate("SHIPMENT INVOICE", this.RefNoFormatted, this.ShipmentRefNo, this.PurchaseOrderRefNo);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task CheckDependency()
        {
            //if (await (from si in _Webcontext.SerialedItems
            //           join im in _Webcontext.Items on si.ItemId equals im.Id
            //           where si.ShipmentInvoiceId == this.Id && im.IsSerialized
            //           select si.Id).AnyAsync())
            //    this.AddMessage("Serialed Items entered");

            //if (await _Webcontext.SerialedItems.AnyAsync(x => x.ShipmentInvoiceId == this.Id && x.ReceivedDate.HasValue))
            //    this.AddMessage("Items Received");
        }
        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("SHIPMENT INVOICE", this.RefNoFormatted, this.ShipmentRefNo, this.PurchaseOrderRefNo);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
