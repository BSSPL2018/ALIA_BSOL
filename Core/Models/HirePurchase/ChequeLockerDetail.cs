using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Models.HirePurchase
{
    public class ChequeLockerDetail
    {
        #region column
        public long ID { get; set; }
        public long? CustomerID { get; set; }
        public long? AgreementID { get; set; }
        public long? OldLockerID { get; set; }
        public int? OldRackNo { get; set; }
        public int? OldBinNo { get; set; }
        public int? OldLevelNo { get; set; }
        public long LockerID { get; set; }
        public int RackNo { get; set; }
        public int BinNo { get; set; }
        public int LevelNo { get; set; }
        public string ChequeStatus { get; set; }
        public DateTime? StatusUpdatedOn { get; set; }
        public string ReceivedBy { get; set; }
        public DateTime? ReceivedOn { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public long? ChequeID { get; set; }
        public string Remarks { get; set; }
        public string PickedBy { get; set; }
        #endregion

        #region Additional COlumns
        [NotMapped]
        public string ChequeNo { get; set; }
        [NotMapped]
        public string CustomerName { get; set; }
        [NotMapped]
        public string AgreementNo { get; set; }
        [NotMapped]
        public string OldLockerName { get; set; }
        [NotMapped]
        public string NewLockerName { get; set; }
        #endregion 
    }
}
