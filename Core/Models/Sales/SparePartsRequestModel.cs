using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Models.Sales
{
    public class SparePartsRequestModel
    {
        public long ID { get; set; }
        public string RefNoFormatted { get; set; }
        public string QuotationNo { get; set; }
        public string Product { get; set; }
        public string CustomerName { get; set; }
        public string ContactNo { get; set; }
        public string DeliverTo { get; set; }
        public string DeliveryAddress { get; set; }
        public string IslandName { get; set; }
        public string AtollName { get; set; }
        public long AtollID { get; set; }
        public long IslandID { get; set; }
        public string CustomerAddress { get; set; }
        public string IslandCode { get; set; }
        public DateTime? RequiredDeliveryOn { get; set; }
        public string VesselName { get; set; }
        public string VesselContact { get; set; }
        public string RequestReceivedBy { get; set; }
        public DateTime? RequestReceivedOn { get; set; }
        public string PaymentReferenceNo { get; set; }
        public DateTime? PaymentReceivedOn { get; set; }
        public string Remarks { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public string BillRefNo { get; set; }
        public decimal? BillAmount { get; set; }
        public string BillEntryBy { get; set; }
        public DateTime? BillEntryDate { get; set; }
        public string PackedEntryBy { get; set; }
        public DateTime? PackedEntryDate { get; set; }
        public string ActualDeliveryAddress { get; set; }
        public string ActualDeliveryTo { get; set; }
        public string AssignedTo { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string DeliveryBy { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryEntryBy { get; set; }
        public DateTime? DeliveryEntryDate { get; set; }
        public string AcknowledgedBy { get; set; }
        public DateTime? AcknowledgedDate { get; set; }
        public string AcknowledgedRemarks { get; set; }
        public bool CanApprove { get; set; }
        public bool CanUndo { get; set; }
        public bool FinalUndo { get; set; }
        public long CustomerID { get; set; }
        public long? AssignedToId { get; set; }
        public long? DeliveryById { get; set; }
        public string ReasonForCancel { get; set; }
        public string RequestReceivedTo { get; set; }
        public string Currency { get; set; }

        [NotMapped]
        public string DeleteColumn { get; set; }
        [NotMapped]
        public string RevertColumn { get; set; }
        [NotMapped]
        public string CheckedColumn { get; set; }
    }
}
