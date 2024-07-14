namespace BSOL.Core.Models.HirePurchase
{
    public class ChequeDepositDateChange
    {
        public long ID { get; set; }
        public long ChequeID { get; set; }
        public string ReasonForDelay { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public DateTime? OldDepositDate { get; set; }
        public DateTime? NewDepositDate { get; set; }
    }
}
