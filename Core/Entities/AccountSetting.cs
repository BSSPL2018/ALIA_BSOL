using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class AccountSetting : BaseObject
    {
        #region Columns
        public long Id { get; set; }
        public int FinancialYearFirstMonth { get; set; }
        public string BaseCurrency { get; set; }
        public long? ReceivableUSDAccountId { get; set; }
        public long? ReceivableMVRAccountId { get; set; }
        public long? PayableUSDAccountId { get; set; }
        public long? PayableMVRAccountId { get; set; }
        public long? GSTPayableAccountId { get; set; }
        public long? IncomeAccountId { get; set; }
        public long? ExpenseAccountId { get; set; }
        public long? InventoryAccountId { get; set; }
        public long? DiscountAccountId { get; set; }
        public long? UnDepositedChequeAccountId { get; set; }
        public long? UnDepositedCashAccountId { get; set; }
        public long? InventoryAccountSupplierId { get; set; }
        public long? InventoryAccountFreightId { get; set; }
        public long? InventoryAccountDutyId { get; set; }
        public long? InventoryAccountOthersId { get; set; }
        public long? MasterShopId { get; set; }
        public long? ServiceChargeAccountId { get; set; }
        public long? ConsignmentInventoryAccountId { get; set; }
        public long? ConsignmentCreditorUSDAccountId { get; set; }
        public long? ConsignmentCreditorMVRAccountId { get; set; }
        #endregion

        protected override async Task Add()
        {
            _Webcontext.AccountSettings.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.AccountSettings.FindAsync(Id);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("ACCOUNT SETTING");
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("ACCOUNT SETTING");
            await _Webcontext.SaveChangesAsync();
        }
    }
}
