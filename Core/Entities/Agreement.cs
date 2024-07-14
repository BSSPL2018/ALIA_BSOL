using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class Agreement
    {
        public long tbl_Id { get; set; }
        public int RefNo { get; set; }
        public string AgrementRef { get; set; }
        public long? BookingID { get; set; }
        public string Proforma_No { get; set; }
        public DateTime? Agrement_Date { get; set; }
        public DateTime? InstallmentStart { get; set; }
        public string EntryBy { get; set; }
        public bool Cancel_flag { get; set; }
        public string Registration_No { get; set; }
        public bool? LR1_Form { get; set; }
        public bool? IsMortgage { get; set; }
        public bool? IsMortgageUnit { get; set; }
        public string Mortgage_AgreeNo { get; set; }
        public string Mortgage_VesselName { get; set; }
        public string Mortgage_VesselType { get; set; }
        public string Mortgage_RegNo { get; set; }
        public DateTime? Mortgage_Agrementdt { get; set; }
        public decimal? MortgageValue { get; set; }
        public string MortgageRemarks { get; set; }
        public double? Staffs_Profit { get; set; }
        public decimal? Staffs_Due { get; set; }
        public decimal? Staffs_Price { get; set; }
        public bool? IsDiscount { get; set; }
        public bool? IsIjara { get; set; }
        public bool? IsMortgageLater { get; set; }
        public DateTime? MortgageLaterDate { get; set; }
        public string CivilCourtRefNo { get; set; }
        public string AttachmentPath { get; set; }
        public string AttachmentApprovedBy { get; set; }
        public DateTime? AttachmentAppprovedDate { get; set; }
        public string SignedAT { get; set; }
        public bool? IsThirdPartyMortgage { get; set; }
        public string OldLockerID { get; set; }
        public int? OldRackNo { get; set; }
        public int? OldBinNo { get; set; }
        public int? OldLevelNo { get; set; }
        public string LockerID { get; set; }
        public int? RackNo { get; set; }
        public int? BinNo { get; set; }
        public int? LevelNo { get; set; }
        public string BinnedBy { get; set; }
        public DateTime? BinnedDate { get; set; }
        public DateTime? PickedDate { get; set; }
        public string? LockerStatus { get; set; }
        public string AgreementType { get; set; }
    }
}
