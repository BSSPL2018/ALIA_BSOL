using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class BillingMaster
    {
        public long tbl_Id { get; set; }
        public string Unit { get; set; }
        public string Customer_Id { get; set; }
        public string InvoiceType { get; set; }
        public string ItemCode { get; set; }
        public string InvoiceNo { get; set; }
        public decimal? DueAmount { get; set; }
        public byte? No_Payment { get; set; }
        public decimal? InitialAmount { get; set; }
        public decimal? GST { get; set; }
        public decimal? TotalPrice { get; set; }
        public byte? Qty { get; set; }
        public string Remarks { get; set; }
        public decimal? BalanceAmount { get; set; }
        public DateTime? Invoice_Date { get; set; }
        public bool Cancel_Flag { get; set; }
        public string ProInv_No { get; set; }
        public string EntryBy { get; set; }
        public decimal? Advance_Amount { get; set; }
        public decimal? USDRate { get; set; }
        public string SoldBy { get; set; }
        public string SalesType { get; set; }
        public string Paymt_Sts { get; set; }
        public DateTime? Paymt_Completedt { get; set; }
        public decimal? TotalPrice_withoutGST { get; set; }
        public decimal? Dealer_Charge { get; set; }
        public decimal? ProfitMargin { get; set; }
        public decimal? surcharge { get; set; }
        public string CustomerType { get; set; }
        public string Currency_Type { get; set; }
        public string IsInternal_Consumption { get; set; }
        public double? GST_Rate { get; set; }
        public DateTime? RegCancelLetterSentDate { get; set; }
        public decimal? DealerDue { get; set; }
        public long? CustID { get; set; }
        public string AttachmentPath { get; set; }
        public string AttachmentApprovedBy { get; set; }
        public DateTime? AttachmentAppprovedDate { get; set; }
        public string PDAttachmentPath { get; set; }
        public string PDAttachmentApprovedBy { get; set; }
        public DateTime? PDAttachmentAppprovedDate { get; set; }
        public string ProformaType { get; set; }
        public string PromotionCode { get; set; }
    }
}
