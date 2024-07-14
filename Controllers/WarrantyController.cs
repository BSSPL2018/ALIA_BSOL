using BSOL.Core;
using BSOL.Helpers;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOL.Core.Models.Common;
using BSOL.Core.Models.Warranty;
using Microsoft.Data.SqlClient;
using Kendo.Mvc.Extensions;
using System.Data;

namespace BSOL.Controllers
{
    public class WarrantyController : BaseController
    {
        public WarrantyController(BSOLContext context, AppUser appUser) : base(context, appUser)
        {
        }

        [HttpPost]
        [ValidateAction(Forms.PosAndMarketting.WarrantyMaster, Rights.View)]
        public async Task<WarrantyDetail> ReadWarrantyDetails([DataSourceRequest] DataSourceRequest Request, [FromForm] string engineNo)
        {
            return await _context.ExecuteSingleAsync<WarrantyDetail>("spPOS_WarrantyDetails", new { Option = "GET_WARRANTY_LIST", engineNo });
        }

        [HttpPost]
        [ValidateAction(Forms.PosAndMarketting.WarrantyMaster, Rights.View)]
        public async Task<List<DropDownModel>> ReadSerialNo([FromForm] string engineNo)
        {
            List<DropDownModel> result = await _context.ExecuteSpAsync<DropDownModel>("spPOS_WarrantyDetails", new { Option = "GET_SERIALNO", engineNo });
            return result;
        }
    }
}
