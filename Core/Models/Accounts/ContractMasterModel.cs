using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Models.Accounts
{
    public class ContractMasterModel
    {
        public long ID { get; set; }
        public string ContractRefNo { get; set; }
        public string? ContractType { get; set; }
        public string? ContractName { get; set; }
        public long VendorID { get; set; }
        public string? Details { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public double ConversonRate { get; set; }
        public DateTime ContractFrom { get; set; }
        public DateTime ContractTo { get; set; }
        public int ExpireNotifyDays { get; set; }
        public string ExpireNotifyTo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? VerifiedBy { get; set; }
        public DateTime? VerifiedOn { get; set; }
        public string? DocumentAttached { get; set; }
        public bool IsActive { get; set; }
        public string? InActiveRemarks { get; set; }
        public string? NotifiedStatus { get; set; }
        public bool? IsRenewal { get; set; }
        public bool? CanApprove { get; set; }
        public bool? CanUndo { get; set; }
        public bool? CanDelete { get; set; }
        public string? CStatus { get; set; }
        public decimal ContractAmount { get; set; }
        public string? SupplierName { get; set; }
    }
}
