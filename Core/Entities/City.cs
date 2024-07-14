using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class City
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int StateID { get; set; }
        public State State { get; set; }
        [NotMapped]
        public int CountryID { get; set; }
    }
}
