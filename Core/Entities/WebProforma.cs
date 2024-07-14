using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class WebProforma
    {
        public long ID { get; set; }
        public string ProformaNo { get; set; }
        public string ProformaType { get; set; }
        public DateTime ProformaDate { get; set; }
        public string Product { get; set; }
        public string ItemCode { get; set; }
        public string Condition { get; set; }
        public string Installation { get; set; }
        public string BoatType { get; set; }
        public string Type { get; set; }
        public int? NoInstallation { get; set; }
        public int? Qty { get; set; }
        public string PurchasorType { get; set; }
        public string CurrencyType { get; set; }
        public decimal USDRate { get; set; }
        public decimal GSTRate { get; set; }
        public decimal UnitsDiscount { get; set; }
        public decimal AccessoriesDiscount { get; set; }
        public decimal CashGST { get; set; }
        public decimal TotalCashPrice { get; set; }
        public decimal InitialPayment { get; set; }
        public int No_Payment { get; set; }
        public decimal DueAmount { get; set; }
        public double ProfitMargin { get; set; }
        public decimal ProfitGST { get; set; }
        public decimal ServiceGST { get; set; }
        public decimal TotalAmount { get; set; }
        public string Remarks { get; set; }
        public string CustomerRefNo { get; set; }
        public string CustomerType { get; set; }
        public string CustomerName { get; set; }
        public string ContactName { get; set; }
        public string NICNo { get; set; }
        public string ContactNo { get; set; }
        public string EmailID { get; set; }
        public string Address { get; set; }
        public string ApprovedBy { get; set; }
        public string RefNo { get; set; }
        public string Atoll { get; set; }
        public string Island { get; set; }
        public string TripID { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public bool? CancelFlag { get; set; }
        public string ProformaStatus { get; set; }
        public string StatusRemark { get; set; }
        public string ReasonForCancel { get; set; }
        public long? EnquiryID { get; set; }
        public string ShowroomReceivedBy { get; set; }
        public DateTime? ShowroomReceivedDate { get; set; }
        public string OfficeReceivedBy { get; set; }
        public DateTime? OfficeReceivedDate { get; set; }
        public string CRMAssignedBy { get; set; }
        public string CRMAssignedTo { get; set; }
        public DateTime? CRMAssignedDate { get; set; }
        public string FormNo { get; set; }
        public string CustomerRemarks { get; set; }
        public DateTime? NextFollowup { get; set; }
    }
}
