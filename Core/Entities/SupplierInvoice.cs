using BSOL.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class SupplierInvoice : BaseObject
    {
        public long Id { get; set; }
        public long ShopId { get; set; }
        public string RefNo { get; set; }
        public long SupplierId { get; set; }
        public long? ShipmentId { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal Amount { get; set; }
        public long? DebitTo { get; set; }
        public long? CreditTo { get; set; }
        public long? GSTSettingId { get; set; }
        public decimal? GSTPercent { get; set; }
        public decimal? GSTAmount { get; set; }
        public long? GSTAccountId { get; set; }
        public decimal Balance { get; set; }
        public string Remarks { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public decimal? OpeningBalance { get; set; }
        public string ExpenseCategory { get; set; }
        public bool POSelected { get; set; }
        public long? GroupReferenceNo { get; set; }
        [NotMapped]
        public string SupplierName { get; set; }
        [NotMapped]
        public string ShopName { get; set; }
        [NotMapped]
        public string Debit { get; set; }
        [NotMapped]
        public string GST { get; set; }
        [NotMapped]
        public long PurchaseOrderID { get; set; }
        [NotMapped]
        public bool Selected { get; set; }
        [NotMapped]
        public long? PaymentId { get; set; }
        [NotMapped]
        public string LocationPath { get; set; }
        [NotMapped]
        public string Comment { get; set; }
        [NotMapped]
        public string PaymentType { get; set; }
        [NotMapped]
        public string ShipmentRefNo { get; set; }
        [NotMapped]
        public string PORefNos { get; set; }
        protected override async Task Add()
        {
            _Webcontext.SupplierInvoices.Add(this);
            //LogAdd("Item");
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Validate()
        {
            if (await _Webcontext.ItemCostings.AnyAsync(x => x.ShipmentId == this.ShipmentId))
                AddMessage("Costing already done for this shipment (" + this.ShipmentRefNo + ")");
        }

        protected override async Task Update()
        {
            var existing = await _Webcontext.SupplierInvoices.FindAsync(Id);
            //_Webcontext.Entry(existing).CurrentValues.SetValues(this);
            // _Webcontext.Entry(existing).Property(x => x.Active).IsModified = false;
            LogUpdate("SupplierInvoices", this.Id, this.RefNo);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            await _Webcontext.SaveChangesAsync();
        }
    }

}
