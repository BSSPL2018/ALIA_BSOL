using Microsoft.EntityFrameworkCore;

namespace BSOL.Core.Entities
{
    public class SupplierBankDetail : BaseObject
    {
        #region Columns
        public long ID { get; set; }
        public long SupplierId { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public long AccountNo { get; set; }
        public string Branch { get; set; }
        public string SwiftCode { get; set; }
        public bool Active { get; set; }
        public int BankId { get; set; }
        public string Address { get; set; }
        public bool IsPrimary { get; set; }
        #endregion

        protected override async Task Validate()
        {
            if (await _Webcontext.SupplierBankDetails.AnyAsync(x => x.ID == this.SupplierId && x.BankName == this.BankName && x.AccountNo == this.AccountNo && x.ID != this.ID))
                AddMessage("Same Account No (" + this.AccountNo + ") already exists");
        }
        protected override async Task Add()
        {
            _Webcontext.SupplierBankDetails.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.SupplierBankDetails.FindAsync(ID);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("SUPPLIER CONTACT", this.BankName,this.AccountName,this.AccountNo);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task CheckDependency()
        {
        }
        protected override async Task Delete()
        {
            var existing = await _Webcontext.SupplierBankDetails.FindAsync(ID);
            existing.Active = false;
            LogDelete("SUPPLIER CONTACT", this.BankName, this.AccountName, this.AccountNo);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
