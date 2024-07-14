namespace BSOL.Core.Entities
{
    public class Country
    {
        public long ID { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string Nationality { get; set; }
        public decimal? DepositAmount { get; set; }
    }
}
