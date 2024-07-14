using Microsoft.AspNetCore.Mvc;
using BSOL.Core.Entities;
using System.Data;
using BSOL.Core;
using BSOL.Core.Models.Common;
using BSOL.Helpers;
using BSOL.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using BSOL.Core.Models.Promotion;
using Kendo.Mvc.UI;
using BSOL.Core.Models.Marketting;
using Kendo.Mvc.Extensions;

namespace BSOL.Controllers
{
    public class MarkettingController : BaseController
    {
        public MarkettingController(BSOLContext context, IConfiguration configuration, AppUser appUser) : base(context, appUser)
        {
        }

        [HttpPost]
        public async Task<DataSourceResult> ReadPromotionList([DataSourceRequest] DataSourceRequest Request)
        {
            List<RamadanPromotionModel> lorryDetails = await _context.ExecuteSpAsync<RamadanPromotionModel>("spPOS_RamazanPromotionList", new { Option = "SELECT" });
            return lorryDetails.ToDataSourceResult(Request);
        }
    }
}
