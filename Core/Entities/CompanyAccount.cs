using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class CompanyAccount : BaseObject
    {
        #region Columns
        public long ID { get; set; }
        public long CompanyID { get; set; }
        public int BankID { get; set; }
        public string BranchName { get; set; }
        public string Currency { get; set; }
        public string CompanyAccountName { get; set; }
        public string CompanyAccountNo { get; set; }
        public decimal BankCharge { get; set; }
        public string ChargeBasedOn { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        #endregion

        #region Additional Properties
        [NotMapped]
        public string BankName { get; set; }
        [NotMapped]
        public string BankCode { get; set; }
        #endregion

        protected override async Task Add()
        {
            _Webcontext.CompanyAccounts.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Validate()
        {
            if (await _Webcontext.CompanyAccounts.AnyAsync(x => x.CompanyID == this.CompanyID && x.BankID == this.BankID && x.ID != this.ID && x.BranchName == this.BranchName && x.Currency == this.Currency && x.CompanyAccountName == this.CompanyAccountName && x.CompanyAccountNo == this.CompanyAccountNo))
                AddMessage("Same Account Name already exists");
            if (await _Webcontext.CompanyAccounts.AnyAsync(x => x.CompanyID == this.CompanyID && x.CompanyAccountNo == this.CompanyAccountNo && x.ID != this.ID))
                AddMessage("Same Account No already exists within the selected Bank");
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.CompanyAccounts.FindAsync(ID);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("CompanyAccounts", this.CompanyAccountName, this.BranchName, this.Currency);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task CheckDependency()
        {
            if (await _Webcontext.Employments.AnyAsync(x => x.CompanyAccountID1 == this.ID || x.CompanyAccountID2 == this.ID))
                AddMessage("This Account assigned to Employee.");
        }
        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("CompanyAccounts", this.CompanyAccountName, this.BranchName, this.Currency);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
