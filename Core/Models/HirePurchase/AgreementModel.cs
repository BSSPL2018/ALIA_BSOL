using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Models.HirePurchase
{
    public class AgreementModel
    {
        public long tbl_Id { get; set; }
        public string? ProRefNo { get; set; }
        public string? Cust_Name_EN { get; set; }
        public string? Product { get; set; }
        public string? ItemCode { get; set; }
        public DateTime? Agrement_Date { get; set; }
        public string? EntryBy { get; set; }
        public string? AgrementRef { get; set; }
        public string? Cust_Ref { get; set; }
        public string? Customer_Type { get; set; }
        public string? Guarantor_Name { get; set; }
        public string? Guarantor_ID { get; set; }
        public DateTime? BinnedDate { get; set; }
        public DateTime? PickedDate { get; set; }
        public string? LockerStatus { get; set; }
        public string? AgreementType { get; set; }
        public DateTime? InstallmentStart { get; set; }
    }
}
