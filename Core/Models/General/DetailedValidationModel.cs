using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Models.General
{
    public class DetailedValidationModel
    {
        public string Category { get; set; }
        public string Message { get; set; }
        [NotMapped]
        public string ActionLink { get; set; }
    }
}
