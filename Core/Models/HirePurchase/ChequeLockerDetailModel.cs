namespace BSOL.Core.Models.HirePurchase
{
    public class ChequeLockerDetailModel
    {
        public long ChequeLockerID { get; set; }
        public long ChequeID { get; set; }
        public string ChequeNo { get; set; }
        public string LockerName { get; set; }
        public int RackNo { get; set; }
        public int BinNo { get; set; }
        public int LevelNo { get; set; }
        public string OldLockerName { get; set; }
        public int? OldRackNo { get; set; }
        public int? OldBinNo { get; set; }
        public int? OldLevelNo { get; set; }
        public string AgrementRef { get; set; }
        public string CustomerName { get; set; }
        public string ChequeStatus { get; set; }
        public DateTime? StatusUpdatedOn { get; set; }
        public string ReceivedBy { get; set; }
        public DateTime? ReceivedOn { get; set; }
        public string EntryBy { get; set; }
        public DateTime? EntryDate { get; set; }
        public long? OldLockerID { get; set; }
        public long LockerID { get; set; }
        public string Remarks { get; set; }
        public string PickedBy { get; set; }
    }
}
