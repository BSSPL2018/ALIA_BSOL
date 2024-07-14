using BSOL.Core;
using BSOL.Core.Models.Common;
using BSOL.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace BSOL.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(BSOLContext context, AppUser appUser) : base(context, appUser)
        {
        }
        [HttpPost]
        public async Task<List<DropDownModel>> ReadShops()
        {
            List<DropDownModel> result = await _context.ExecuteSpAsync<DropDownModel>("spPORTAL_ShopList", new { Options = "GET_SHOP" });
            return result;
        }
    }
}
