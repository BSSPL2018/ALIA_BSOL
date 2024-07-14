using BSOL.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using Telerik.Reporting.Cache.File;
using Telerik.Reporting.Processing;
using Telerik.Reporting.Services;
using Telerik.Reporting.Services.AspNetCore;

namespace BSOL.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("reports")]
    public class ReportsController : ReportsControllerBase
    {
        public ReportsController(IReportServiceConfiguration reportServiceConfiguration, IConfiguration configuration, IWebHostEnvironment environment):base(reportServiceConfiguration)
        {
            reportServiceConfiguration.HostAppId = configuration.GetValue<string>("HostAppId");
            reportServiceConfiguration.Storage = new FileStorage();

            string reportsPath = configuration.GetValue<string>("ReportPath");
            if (!reportsPath.IsValid())
                reportsPath = Path.Combine(environment.ContentRootPath, "Reports");

            reportServiceConfiguration.ReportSourceResolver = new UriReportSourceResolver(reportsPath);
        }

        public override IActionResult GetParameters(string clientID, [FromBody] ClientReportSource reportSource)
        {
            var encryptedParams = reportSource.ParameterValues.Keys.Where(x => x.StartsWith("ENC_")).ToList();
            foreach (var key in encryptedParams)
            {
                var value = Convert.ToString(reportSource.ParameterValues[key]);
                reportSource.ParameterValues.Remove(key);
                value = Utilities.DecryptParam(value);
                reportSource.ParameterValues.Add(key.Replace("ENC_", ""), value);
            }
            return base.GetParameters(clientID, reportSource);
        }

        protected override UserIdentity GetUserIdentity()
        {
            var identity = base.GetUserIdentity();
            identity.Name = HttpContext.User.FindFirst(ClaimType.ShortName.ToString())?.Value;
            return identity;
        }
    }
}