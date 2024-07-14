using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class Customer : BaseObject
    {
        #region Columns
        public long ID { get; set; }
        public string NicNo { get; set; }
        public DateTime? NicExpiredOn { get; set; }
        public string NameEN { get; set; }
        public string NameDHI { get; set; }
        public string PermanentAddressEN { get; set; }
        public string PermanentAddressDHI { get; set; }
        public string PresentAddressEN { get; set; }
        public string PresentAddressDHI { get; set; }
        public DateTime? DOB { get; set; }
        public string Nationality { get; set; }
        public string Gender { get; set; }
        public string ContactNo { get; set; }
        public string MobileNo { get; set; }
        public string EmailID { get; set; }
        public string FaxNo { get; set; }
        public string TINNo { get; set; }
        public int? WorkedMonth { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public string CustomerType { get; set; }
        public string EmployerType { get; set; }
        public string EmployerName { get; set; }
        public string EmployerDetails { get; set; }
        public string EmployerPresentAddressEN { get; set; }
        public string EmployerPresentAddressDHI { get; set; }
        public string EmployerContactNo { get; set; }
        public string EmergencyContactNameEN { get; set; }
        public string EmergencyContactNameDHI { get; set; }
        public string EmergencyContactNo { get; set; }
        public long? IslandID { get; set; }
        public string Remarks { get; set; }
        public string EntryBy { get; set; }
        public DateTime? EntryDate { get; set; }
        public bool Active { get; set; }
        public bool? IsHPCustomer { get; set; }
        public string ApprovedBy { get; set; }
        public long? CompanyId { get; set; }

        public DateTime? ApprovedDate { get; set; }

        #endregion
        #region Additional Columns
        [NotMapped]
        public string Atoll { get; set; }
        [NotMapped]
        public string Island { get; set; }
        [NotMapped]
        public long AtollID { get; set; }
        [NotMapped]
        public string RegistrationType { get; set; }
        #endregion

        protected override async Task Validate()
        {
            if (await _Webcontext.Customers.Where(x => x.Active).AnyAsync(x => x.NicNo == this.NicNo))
                AddMessage("Same NIC No (" + this.NicNo + ") already exists");
        }
        protected override async Task Add()
        {
            _Webcontext.Customers.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.Customers.FindAsync(ID);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);

            LogUpdate("NIC No", this.NicNo);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Delete()
        {
            var existing = await _Webcontext.Customers.FindAsync(ID);
            existing.Active = false;
            LogDelete("NIC No", this.NicNo);
            await _Webcontext.SaveChangesAsync();
            await UpdateCustomerStatus(ID);
        }
        protected override async Task CheckDependency()
        {
            if (await _Webcontext.Customers.AnyAsync(x => x.ID == this.ID && x.ApprovedDate != null))
                AddMessage("Customer was approved.");
        }
        private async Task UpdateCustomerStatus(long ID)
        {
            await _Webcontext.ExecuteNonQueryAsync("spFMS_Customer", new
            {
                Option = "UPDATE_CUSTOMER_STATUS",
                ID = ID
            });
        }
        private async Task ApproveCustomerStatus(long ID)
        {
            await _Webcontext.ExecuteNonQueryAsync("spFMS_Customer", new
            {
                Option = "APPROVED_CUSTOMER_STATUS",
                ID = ID
            });
        }
        protected override async Task<bool> OnApprove()
        {
            var existing = await _Webcontext.Customers.FindAsync(ID);

            existing.ApprovedBy = base.GetAppUserName();
            existing.ApprovedDate = DateTime.Now;

            Log("Customer", ActionType.Approve, existing.NicNo);
            await _Webcontext.SaveChangesAsync();
            await ApproveCustomerStatus(ID);
            return true;
        }
        protected override async Task<bool> OnReject()
        {
            var existing = await _Webcontext.Customers.FindAsync(ID);

            existing.ApprovedBy = null;
            existing.ApprovedDate = null;

            Log("Customer", ActionType.Reject, existing.NicNo);
            await _Webcontext.SaveChangesAsync();
            await ApproveCustomerStatus(ID);
            return true;
        }
    }

}
