using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class ChequeRegistry
    {
        public long tbl_Id { get; set; }
        public string Agreement_Ref { get; set; }
        public long? Agreement_Id { get; set; }
        public string BankName { get; set; }
        public string Account_Name { get; set; }
        public string AccountNo { get; set; }
        public string Cheque_No { get; set; }
        public decimal? Due_Amount { get; set; }
        public DateTime? Due_Date { get; set; }
        public string Received_By { get; set; }
        public decimal PaidAmount { get; set; }
        public string Payment_Mode { get; set; }
        public string Payment_EntryBy { get; set; }
        public string Cheque_Verified { get; set; }
        public DateTime? Cheque_EntryDate { get; set; }
        public string Remarks { get; set; }
        public bool? Cancel_Flag { get; set; }
        public DateTime? Send_to_Bank { get; set; }
        public decimal? Cheque_Amount { get; set; }
        public string Letter_ReqBy { get; set; }
        public string InvoiceNo { get; set; }
        public bool? Send { get; set; }
        public string Cheque_SendBy { get; set; }
        public DateTime? ChequeSendDate { get; set; }
        public bool? Received { get; set; }
        public string Cheque_ReceivedBy { get; set; }
        public string Currency_Type { get; set; }
        public bool Paid { get; set; }
        public DateTime? PaidDate { get; set; }
        public string Postedby { get; set; }
        public DateTime? PostedDate { get; set; }
        public decimal? FCI { get; set; }
        public decimal? CPI { get; set; }
        public decimal? GST_FCI { get; set; }
        public decimal? GST_CPI { get; set; }
        public decimal? OC { get; set; }
        public string ChequeHandoverBy { get; set; }
        public DateTime? ChequeHandoverDate { get; set; }
        public DateTime? BounceDate { get; set; }
        public string ChequeBounceReason { get; set; }
        public string BounceUpdatedBy { get; set; }
        public DateTime? BounceUpatedDate { get; set; }
        public DateTime? BounceChequePrintDate { get; set; }
    }
}
