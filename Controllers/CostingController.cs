using BSOL.Controllers;
using BSOL.Core;
using BSOL.Core.Entities;
using BSOL.Core.Helpers;
using BSOL.Core.Models.Common;
using BSOL.Core.Models.General;
using BSOL.Core.Models.Procurement;
using BSOL.Helpers;
using BSOL.Web.Helpers;
using DocumentFormat.OpenXml.InkML;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Web.Controllers
{
    [Authorize]
    public class CostingController : BaseController
    {
        private readonly ICommonHelper _commonHelper;
        public CostingController(BSOLContext context,BSOLWebContext Webcontext, AppUser appUser, ICommonHelper commonHelper) : base(context,Webcontext, appUser)
        {
            _commonHelper = commonHelper;
        }

        #region Costing
        [ValidateAction(Forms.Procurement.Costing, Rights.View)]
        public async Task<DataSourceResult> ReadShipmentsForCosting([DataSourceRequest] DataSourceRequest request, [FromForm] int statusFilter = 0)
        {
            List<ItemCostingMasterModel> result;
            if(statusFilter == 0)
                result = await _Webcontext.ExecuteSpAsync<ItemCostingMasterModel>("spBSOL_SelectItemCostingPending", new { _appUser.CompanyId });
            else
                result = await _Webcontext.ExecuteSpAsync<ItemCostingMasterModel>("spBSOL_SelectItemCostingCompleted", new { _appUser.CompanyId, statusFilter });

            return await result.ToDataSourceResultAsync(request);
        }
        [ValidateAction(Forms.Procurement.Costing, Rights.View)]
        public async Task<DataSourceResult> ReadItemCosting([DataSourceRequest] DataSourceRequest request, [FromForm] long? ShipmentId = 0, [FromForm] long? purchaseOrderId = 0)
        {
            if (ShipmentId > 0)
            {
                var result = await _Webcontext.ExecuteSpAsync<ItemCostingModel>("spBSOL_ItemCostingForShipment", new { ShipmentId });
                return result.ToDataSourceResult(request);
            }
            else
            {
                var result = await _Webcontext.ExecuteSpAsync<ItemCostingModel>("spBSOL_ItemCostingForLocalPurchase", new { purchaseOrderId });
                return result.ToDataSourceResult(request);
            }
        }
        [ValidateAction(Forms.Procurement.Costing, Rights.Modify)]
        public async Task<ReturnMessage> SaveItemCosting(List<ItemCosting> itemCostings, [FromQuery] long? shipmentId, [FromQuery] long? purchaseOrderId, [FromQuery] string refNo)
        {
            var tableSchema = new List<SqlMetaData>() {
                new SqlMetaData("ItemId", SqlDbType.BigInt),
                new SqlMetaData("ConfirmedQty", SqlDbType.Decimal,18,2),
                new SqlMetaData("CustomDutyFee", SqlDbType.Money),
                new SqlMetaData("Freight", SqlDbType.Money),
                new SqlMetaData("OtherExpenses", SqlDbType.Money),
                new SqlMetaData("PurchasedRate", SqlDbType.Money),
                new SqlMetaData("OldCostOfGoods", SqlDbType.Money),
                new SqlMetaData("CostOfGoods", SqlDbType.Money),
                new SqlMetaData("WAC", SqlDbType.Money),
                new SqlMetaData("SellingRate", SqlDbType.Money),
                new SqlMetaData("OldSellingRate", SqlDbType.Money),
                new SqlMetaData("OldQty", SqlDbType.Decimal,18,2),
                new SqlMetaData("CostingPercentage", SqlDbType.Float)
            }.ToArray();
            var sqlDataRecords = new List<SqlDataRecord>();
            itemCostings.ForEach(x =>
            {
                SqlDataRecord sqlDataRecord = new SqlDataRecord(tableSchema);
                sqlDataRecord.SetInt64(0, x.ItemId);
                sqlDataRecord.SetDecimal(1, x.ConfirmedQty);
                sqlDataRecord.SetDecimal(2, x.CustomDutyFee);
                sqlDataRecord.SetDecimal(3, x.Freight);
                sqlDataRecord.SetDecimal(4, x.OtherExpenses);
                sqlDataRecord.SetDecimal(5, x.PurchasedRate);
                sqlDataRecord.SetDecimal(6, x.OldCostOfGoods);
                sqlDataRecord.SetDecimal(7, x.CostOfGoods);
                sqlDataRecord.SetDecimal(8, x.WAC);
                sqlDataRecord.SetDecimal(9, x.SellingRate);
                sqlDataRecord.SetDecimal(10, x.OldSellingRate);
                sqlDataRecord.SetDecimal(11, x.OldQty);
                sqlDataRecord.SetDouble(12, x.CostingPercentage);
                sqlDataRecords.Add(sqlDataRecord);
            });

            List<SqlParameter> sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@Option", "VAL_SAVE"),
                new SqlParameter("@ShipmentId", shipmentId),
                new SqlParameter("@PurchaseOrderId", purchaseOrderId),
                new SqlParameter("@ShopId", _appUser.ShopId),
                new SqlParameter("@CreatedBy", _appUser.CommonEmpNo),
                new SqlParameter("@CompanyId", _appUser.CompanyId),
                new SqlParameter
                {
                    ParameterName = "@ItemCostingType",
                    Value = sqlDataRecords,
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.ItemCostingType"
                }
            };

            var errors = await _Webcontext.ValidateSqlAsync("spBSOL_ItemCosting", sqlParams);
            if (errors.Any())
                return SaveError(errors);

            sqlParams[0].Value = "INSERT";

            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("spBSOL_ItemCosting", sqlParams);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            var id = shipmentId.HasValue && shipmentId.Value > 0 ? shipmentId.Value : (purchaseOrderId ?? 0);
            await EventLog("COSTING", ActionType.Add.ToString(), "Costing", id, refNo);
            return Message("Item costing details saved successfully.");
        }
        [ValidateAction(Forms.Procurement.Costing, Rights.Delete)]
        public async Task<ReturnMessage> DeleteItemCosting([FromForm] long shipmentId, [FromForm] long purchaseOrderId, [FromForm] string ShipmentRefNo)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_ItemCosting", new { Option = "VAL_DELETE", shipmentId, purchaseOrderId });
            if (errors.Any())
                return DeleteError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_ItemCosting", new
            {
                Option = "DELETE",
                shipmentId,
                purchaseOrderId
            });
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("COSTING", ActionType.Delete.ToString(), "Costing", shipmentId, ShipmentRefNo);
            return Message("Item costing deleted.");
        }
        [ValidateAction(Forms.Procurement.Costing, Rights.Approve)]
        public async Task<ReturnMessage> VerifyItemCosting([FromForm] long shipmentId, [FromForm] long purchaseOrderId, [FromForm] string refNo)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_ItemCosting", new { Option = "VAL_VERIFY", shipmentId, purchaseOrderId, CreatedBy = _appUser.CommonEmpNo });
            if (errors.Any())
                return ApproveError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_ItemCosting", new
            {
                Option = "VERIFY",
                shipmentId,
                purchaseOrderId,
                CreatedBy = _appUser.CommonEmpNo
            });
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("COSTING", ActionType.Approve.ToString(), "Costing", shipmentId, refNo);
            return Message("Item costing verified.");
        }
        [ValidateAction(Forms.Procurement.Costing, Rights.Approve)]
        public async Task<ReturnMessage> RevertItemCosting([FromForm] long shipmentId, [FromForm] long purchaseOrderId, [FromForm] string refNo)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_ItemCosting", new { Option = "VAL_REVERT", ShipmentId = shipmentId, PurchaseOrderId = purchaseOrderId, CreatedBy = _appUser.CommonEmpNo });
            if (errors.Any())
                return UndoError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_ItemCosting", new
            {
                Option = "REVERT",
                shipmentId,
                purchaseOrderId
            });
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("COSTING", ActionType.Reject.ToString(), "Costing", shipmentId, refNo);
            return Message("Item costing un-verified.");
        }
        public async Task<ReturnMessage> ExportCosting([FromForm] long ShipmentId, [FromForm] long purchaseOrderId, [FromForm] string refNo)
        {
            await EventLog("COSTING", ActionType.Export.ToString(), "Costing", ShipmentId, purchaseOrderId, refNo);
            return Message("Ok");
        }

        #endregion
    }
}