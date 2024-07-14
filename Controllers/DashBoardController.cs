using BSOL.Core;
using BSOL.Core.Models.Accounts;
using BSOL.Core.Models.Common;
using BSOL.Core.Models.DashBoard;
using BSOL.Core.Models.HirePurchase;
using BSOL.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BSOL.Controllers
{
    public class DashBoardController : BaseController
    {
        private readonly ICommonHelper _commonHelper;
        public DashBoardController(BSOLContext context, BSOLWebContext webContext, AppUser appUser, ICommonHelper commonHelper) : base(context, webContext, appUser)
        {
            _commonHelper = commonHelper;
        }
        [HttpPost]
        [ValidateAction(Forms.DashBoard.SalesStaus, Rights.View)]
        public async Task<DataSourceResult> ReadSalesStatus([DataSourceRequest] DataSourceRequest request, [FromForm] DateTime FromDate, [FromForm] DateTime ToDate, [FromForm] string Product)
        {
            var result = await _context.ExecuteSpAsync<SalesStatus>("spPOS_General", new { sCase = "SALES_STATUS", Unit = Product, From = FromDate, To = ToDate });
            return await result.ToDataSourceResultAsync(request);
        }
        [HttpPost]
        public async Task<List<StringTypeModel>> ReadProduct()
        {
            return await _context.ShipmentSerialNos.Select(x => new StringTypeModel { Value = x.ProductCode }).Distinct().ToListAsync();
        }
        [HttpPost]
        [ValidateAction(Forms.HirePurchases.HPMonthlyStatus, Rights.View)]
        public async Task<DataSourceResult> ReadUnitStatus([DataSourceRequest] DataSourceRequest request, [FromForm] string Product = null, [FromForm] string CustomerId = null, [FromForm] bool Shipment = false, [FromForm] bool AdvanceBooking = false, [FromForm] bool CreditEvaluation = false, [FromForm] bool Agreement = false, [FromForm] bool HPSales = false, [FromForm] bool CashSales = false, [FromForm] bool HPProcess = false)
        {
            var result = await _context.ExecuteSpAsync<UnitStatus>("spPOS_UnitStatus", new { ProductIds  = Product, CustomerIds = CustomerId, Shipment, AdvanceBooking, CreditEvaluation, Agreement, CashSales, HPSales,HPProcess });
            return await result.ToDataSourceResultAsync(request);
        }
    }
}
