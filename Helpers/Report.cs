using BSOL.Core;
using System.Threading.Tasks;
namespace BSOL.Helpers
{
    public class Report
    {
        public string Category { get; set; }
        public string Title { get; set; }
        public string ReportName { get; set; }
        public bool FromDate { get; set; }
        public bool ToDate { get; set; }
        public bool Shop { get; set; }
        public bool HasAcess { get; set; }
        public int PreviewMode { get; set; }
        public ReportType ReportType { get; set; }
        public bool Product { get; set; }

        public Report()
        {
            this.HasAcess = true;
            this.ReportType = ReportType.Report;
        }

        public Report(AppUser appUser, string formName)
        {
            this.HasAcess = formName.IsValid() ? appUser.HasViewAccess(formName) : false;
            this.ReportType = ReportType.Report;
        }
    }
}
