
using BSOL.Helpers;

namespace BSOL.Core.Models.HirePurchase
{
    public class MonthlyHPStatusModel
    {
        public string ProformaNo { get; set; }
        public DateTime? ProformaDate { get; set; }
        public string? CustomerName { get; set; }
        public string ProformaType { get; set; }
        public string Product { get; set; }
        public string ItemCode { get; set; }
        public decimal? TotalCashPrice { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? ProformaStatus { get; set; }
        public string? AgreementNo { get; set; }
        //public int? M1 { get; set; }
        //public int? M2 { get; set; }
        //public int? M3 { get; set; }
        //public int? M4 { get; set; }
        //public int? M5 { get; set; }
        public string? AgreementType { get; set; }

        public string? FormNo { get; set; }
        public string? FrameNo { get; set; }
        public string? BookedBy { get; set; }
        public DateTime? BookedOn { get; set; }

        public string? EvaluatedBy { get; set; }
        public DateTime? EvaluatedOn { get; set; }
        public string? VerifiedBy { get; set; }
        public DateTime? VerifiedOn { get; set; }

        public string? AGREntyBy { get; set; }
        public DateTime? AGREntryOn { get; set; }

        public string? BilledBy { get; set; }
        public DateTime? SoldDate { get; set; }
        public int? No_Payment { get; set; }
        public decimal? DueAmount { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? BalanceAmount { get; set; }
        public string? SoldBy { get; set; }
        public string BilledUnits { get; set; }
        public long? tbl_Id { get; set; }

        public int ProformaCNT { get { return ProformaNo.IsValid() ? 1 : 0; } }
        public int BookedCNT { get { return BookedBy.IsValid() ? 1 : 0; } }
        public int CreditEvaluationCNT { get { return EvaluatedBy.IsValid() ? 1 : 0; } }
        public int AgreementCNT { get { return AgreementNo.IsValid() ? 1 : 0; } }
        public int BilledCNT { get { return BilledBy.IsValid() ? 1 : 0; } }

    }
}
