using BSOL.Core;
using BSOL.Helpers;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOL.Core.Models.HirePurchase;
using Kendo.Mvc.Extensions;
using BSOL.Core.Models.Common;

namespace BSOL.Controllers
{
    public class CourtCaseListController : BaseController
    {
        public CourtCaseListController(BSOLContext context, AppUser appUser) : base(context, appUser)
        {
        }

        [HttpPost]
        public async Task<DataSourceResult> ReadVerdictPendingList([DataSourceRequest] DataSourceRequest Request, [FromForm] string? NicNo = "")
        {
            List<CourtCaseListModel> CourtCaseListDetails = await _context.ExecuteSpAsync<CourtCaseListModel>("spHP_VerdictPendingList", new { Options = "GET_VERDICT_LIST", NicNo = NicNo });
            return CourtCaseListDetails.ToDataSourceResult(Request);
        }

        [HttpPost]
        public async Task<DataSourceResult> ReadFollowupList([DataSourceRequest] DataSourceRequest Request, [FromForm] string InvoiceNo)
        {
            List<CustomerFollowup> CustomerFollowupDetails = await _context.ExecuteSpAsync<CustomerFollowup>("spHP_VerdictPendingList", new { Options = "GET_FOLLOWUP_LIST", InvoiceNo = InvoiceNo });
            return CustomerFollowupDetails.ToDataSourceResult(Request);
        }
    }
}


