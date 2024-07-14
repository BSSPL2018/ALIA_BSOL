using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class CustomerDetail
    {
        public long tbl_ID { get; set; }
        public string Customer_Id { get; set; }
        public string Cust_Ref { get; set; }
        public string Cust_PresentAddress_En { get; set; }
        public string Cust_PresentAddress_Dhi { get; set; }
        public string Cust_ContactNo { get; set; }
        public string Cust_MobileNo { get; set; }
        public string Cust_Type { get; set; }
        public string Emergency_ContactNo { get; set; }
        public string EmployerName { get; set; }
        public string EmployerType { get; set; }
        public string EmployementDetails { get; set; }
        public string Employer_PresAddress_En { get; set; }
        public string Employer_ContactNo { get; set; }
        public string EntryBy { get; set; }
        public string Remarks { get; set; }
        public bool Cancel_Flag { get; set; }
        public DateTime? EntryDate { get; set; }
        public int? Worked_Month { get; set; }
        public decimal? Monthly_Income { get; set; }
        public string Cust_Name_EN { get; set; }
        public string Cust_Name_DHI { get; set; }
        public string Cust_Per_Address_EN { get; set; }
        public string Cust_Per_Address_DHI { get; set; }
        public string EmailID { get; set; }
        public string FaxNo { get; set; }
        public string Emergency_ContactName { get; set; }
        public string Emergency_ContactName_Dhi { get; set; }
        public string Customer_TIN { get; set; }
    }
}
