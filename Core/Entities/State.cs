namespace BSOL.Core.Entities
{
    public class State
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public long CountryID { get; set; }
        public Country Country { get; set; }
    }
}
