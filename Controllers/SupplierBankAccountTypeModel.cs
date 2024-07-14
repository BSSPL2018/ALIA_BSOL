namespace BSOL.Controllers
{
    public class SupplierBankAccountTypeModel
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public bool IsPrimary { get; set; }
        public string AccountName { get; set; }
        public long AccountNo { get; set; }
    }
}
