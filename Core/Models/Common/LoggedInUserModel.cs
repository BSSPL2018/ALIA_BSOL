using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Models.Common
{
    public class LoggedInUserModel
    {
        public string CommanName { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string Designation { get; set; }
        public string Division { get; set; }
        public string Category { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string LoginID { get; set; }
        public string PassCode { get; set; }
        public string EmpID { get; set; }
        public bool? ACTIVE { get; set; }
        public long UserCode { get; set; }
        public string Machine { get; set; }
        public string Superior { get; set; }
        public bool IsPowerUser { get; set; }
        public bool? IsDivIncharge { get; set; }
        public bool? IsDeptIncharge { get; set; }
        public bool? IsAstIncharge { get; set; }
        public long UserID { get; set; }
        public long EmployeeID { get; set; }
    }
}
