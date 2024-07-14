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

namespace BSOL.wwwroot
{
    public class PromotionController : BaseController
    {
        private readonly IConfiguration _config;
        public PromotionController(BSOLContext context, IConfiguration configuration, AppUser appUser) : base(context, appUser)
        {
            _config = configuration;
        }

        [HttpPost]
        public async Task<ReturnMessage> SaveRamadanPromotion([FromForm] string NicNo, [FromForm] string OutletID,[FromForm] string CustomerName)
        {
            string couponCode = "";
            int couponCodeLength = _config.GetValue<int>("CouponCodeLength");
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            do
            {
                couponCode = new string(Enumerable.Repeat(chars, couponCodeLength).Select(s => s[random.Next(s.Length)]).ToArray());

            } while (await _context.RamazanPromotions.AnyAsync(x => x.CouponCode == couponCode));

            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "VAL_SAVE"),
                        new SqlParameter("@NicNo",NicNo),
                        new SqlParameter("@OutletID", OutletID),
                        new SqlParameter("@couponCode", couponCode),
                        new SqlParameter("@CustomerName", CustomerName),
                    };

            var lstError = await _context.ValidateSqlAsync("spPOS_RamazanPromotionList", sqlParams);
            if (lstError.Any())
                return Error(lstError);

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.AddRange(sqlParams.Select(x => ((ICloneable)x).Clone() as SqlParameter));
            sqlParameters[0].Value = "INSERT";

            var id = await _context.ExecuteSqlNonQueryWithReturnAsync<long>("spPOS_RamazanPromotionList", sqlParameters);

            if (id <= 0)
                return Error();

            return Message(couponCode);
        }

        [HttpPost]
        public async Task<PromotionCustomer> GetCustomer([FromForm] string OutletID)
        {
            return await _context.ExecuteSingleAsync<PromotionCustomer>("spPOS_RamazanPromotionList", new { Option = "GET_CUSTOMER_DETAIL", OutletID });
        }
        
    }
}
