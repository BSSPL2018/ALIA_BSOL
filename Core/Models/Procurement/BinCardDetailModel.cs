namespace BSOL.Core.Models.Procurement
{
    public class BinCardDetailModel
	{
        public DateTime? TransactionDate { get; set; }
        public string Process { get; set; }
        public string RefNo { get; set; }
        public string Unit { get; set; }
        public decimal Qty { get; set; }
        public decimal Stock { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Value { get; set; }
        public decimal WAC { get; set; }
        public decimal InventoryValue { get; set; }

    }
}
