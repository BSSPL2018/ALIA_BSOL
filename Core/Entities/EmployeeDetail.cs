using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class EmployeeDetail
    {
        //public long ID { get; }
        [Key]
        public long EmployeeId { get; set; }
        public string CommonEmpNo { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        [NotMapped]
        public bool Selected { get; set; }
        public string EmpID { get; set; }


    }
}
