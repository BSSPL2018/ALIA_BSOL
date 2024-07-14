using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Models.HirePurchase
{
    public class CourtCaseListModel
    {
        public string? Cust_Ref { get; set; }
        public string? Cust_Name_EN { get; set; }
        public string? Unit { get; set; }
        public string? ItemCode { get; set; }
        public string? Cust_Name_DHI { get; set; }
        public decimal? TotalPrice { get; set; }
        public byte? No_Payment { get; set; }
        public DateTime? CaseFiled { get; set; }
        public decimal? DueBalance { get; set; }
        public Int32? SMonthsPaid { get; set; }
        public Int32? MonthsNotPaid { get; set; }
        public string? InvoiceNo { get; set; }
        public decimal? Settled { get; set; }
        public string? Reference_No { get; set; }
        public DateTime? Invoice_Date { get; set; }
        public decimal? BalanceAmount { get; set; }
        public decimal? DueAmount { get; set; }
    }
}
