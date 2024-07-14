using BSOL.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class Shipment : BaseObject
    {
        #region Columns
        [JsonConverter(typeof(IdentityEncryptor))]
        [ModelBinder(typeof(IdentityDecryptor))]
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public long RefNo { get; set; }
        public string ShipmentMode { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ActualArrivalDate { get; set; }
        public DateTime? OffLoadedToPortDate { get; set; }
        public DateTime? DemurrageDate { get; set; }
        public DateTime? ClearedDate { get; set; }
        public string ClearanceType { get; set; }
        public string CustomsRefNo { get; set; }
        public string LCNo { get; set; }
        public string TradeTerm { get; set; }
        public string Location { get; set; }
        public string PortOfOrigin { get; set; }
        public string PortOfLoading { get; set; }
        public string ContainerLoadType { get; set; }
        public int? Feet20 { get; set; }
        public int? Feet40 { get; set; }
        public string ContainerNo { get; set; }
        public string FirstCarrierVessel { get; set; }
        public string SecondCarrierVessel { get; set; }
        public string Notes { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string RefNoFormatted { get; set; }
        #endregion

        #region Addition Column
        [NotMapped]
        public decimal TotalExpenseAmount { get; set; }
        [NotMapped]
        public string VerifiedBy { get; set; }
        [NotMapped]
        public DateTime? VerifiedOn { get; set; }
        #endregion

        protected override async Task Validate()
        {
        }
        protected override async Task Add()
        {
            long lastRefNo = await _Webcontext.Shipments.Where(x => x.CompanyId == this.CompanyId && x.CreatedOn.Year == DateTime.Now.Year).MaxAsync(x => (long?)x.RefNo) ?? 0;
            this.RefNo = ++lastRefNo;
            this.Active = true;
            _Webcontext.Shipments.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.Shipments.FindAsync(Id);
            if (existing.ClearedDate.HasValue)
            {
                this.AddMessage("Shipment Cleared");
                return;
            }
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            _Webcontext.Entry(existing).Property(x => x.CompanyId).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.CreatedBy).IsModified = false;
            _Webcontext.Entry(existing).Property(x => x.CreatedOn).IsModified = false;
            LogUpdate("SHIPMENT", this.RefNoFormatted);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task CheckDependency()
        {
            if (await _Webcontext.ShipmentInvoices.AnyAsync(x => x.ShipmentId == this.Id))
                AddMessage("Invoice entered for this Shipment");
        }
        protected override async Task Delete()
        {
            //_Webcontext.Remove(this);
            //LogDelete("SHIPMENT", this.RefNoFormatted);
            //await _Webcontext.SaveChangesAsync();
        }
    }
}
