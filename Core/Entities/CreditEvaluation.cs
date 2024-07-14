using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class CreditEvaluation
    {
        public long tblEvl_ID { get; set; }
        public string CustomerRef { get; set; }
        public string CustomerType { get; set; }
        public string Guarantor_ID { get; set; }
        public string Guarantor_Name { get; set; }
        public int? WorkedMonth { get; set; }
        public double? MonthlyIncome { get; set; }
        public string Guarantor_ContactNo { get; set; }
        public string Product { get; set; }
        public string Emergency_ContactID { get; set; }
        public string Emergency_ContName { get; set; }
        public string Emergency_ContactNo { get; set; }
        public string Addtional_Ref_Persnl_ID { get; set; }
        public string Addtional_Ref_Name { get; set; }
        public string Add_Ref_ContactNo { get; set; }
        public string EntryBy { get; set; }
        public DateTime? EntryDate { get; set; }
        public string BookingId { get; set; }
        public string GuarantorPre_Addresss_EN { get; set; }
        public string GuarantorPre_Address_DHI { get; set; }
        public string TGuarantorID { get; set; }
        public string TGuarantorName { get; set; }
        public string TGuarantorPreAddressEN { get; set; }
        public string TGuarantorPreAddressDHI { get; set; }
        public string TGuarantorContactNo { get; set; }
        public int? TGuarantorWorkedMonth { get; set; }
        public decimal? TGuarantorIncome { get; set; }
        public string Remark { get; set; }
        public string Asset { get; set; }
        public string SourceOfIncome { get; set; }
        public decimal? MonthlyExpense { get; set; }
        public string KYCDocument { get; set; }
        public bool? Status { get; set; }
        public string Form_No { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public bool? CancelFlag { get; set; }
    }
}
