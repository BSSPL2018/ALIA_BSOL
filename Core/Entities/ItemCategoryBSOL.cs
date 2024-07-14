namespace BSOL.Core.Entities
{
    public class ItemCategoryBSOL
    {
        public string ItemCategoryName { get; set; }
        public string ItemCategoryCode { get; set; }
        public double? MaxCostofGoods { get; set; }
        public double? MaxFreightAmt { get; set; }
        public double? MaxInsuranceAmt { get; set; }
        public bool? TranShipment { get; set; }
        public int ItemCategory_ID { get; set; }
        public bool? Sno_Entry { get; set; }
        public double? DutyAmountPercentage { get; set; }
        public bool? Make_Inventory { get; set; }
        public bool? Assemble { get; set; }
        public bool? LorryBundle { get; set; }
        public string ItemCategoryCodeDHI { get; set; }
        public bool? PDChequeRequired { get; set; }
    }
}
