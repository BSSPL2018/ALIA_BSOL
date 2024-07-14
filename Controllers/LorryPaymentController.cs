using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOL.Core;
using BSOL.Core.Models.Common;
using BSOL.Core.Models.Logitics;
using Microsoft.Data.SqlClient;
using BSOL.Helpers;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Data;
using System.Net.Mime;

namespace BSOL.Controllers
{
    public class LorryPaymentController : BaseController
    {
        public LorryPaymentController(BSOLContext context, AppUser appUser) : base(context, appUser)
        {
        }

        #region Lorry Payment

        [HttpPost]
        [ValidateAction(Forms.Logistics.LorryPayment, Rights.Delete)]
        public async Task<ReturnMessage> DeleteLorryPayment([FromForm] long id, [FromForm] string product, [FromForm] string shipmentId, [FromForm] decimal Amount, [FromForm] decimal TotalAmount)
        {
            var errors = await _context.ValidateAsync("spLMS_LorryPayments", new { Option = "VAL_DELETE", id });
            if (errors.Any())
                return DeleteError(errors);

            int retVal = await _context.ExecuteNonQueryAsync("spLMS_LorryPayments", new { Option = "DELETE", id });
            if (retVal <= 0)
                return Error();

            await EventLog("Logistics - Lorry Payment", "DELETED", product, shipmentId, Amount, TotalAmount);
            return Message("Payment was cancelled.");
        }

        [HttpPost]
        [ValidateAction(Forms.Logistics.LorryPayment, Rights.Approve)]
        public async Task<ReturnMessage> ApproveLorryPayment([FromForm] long id, [FromForm] string product, [FromForm] string shipmentId, [FromForm] decimal Amount, [FromForm] decimal TotalAmount)
        {
            int retVal = await _context.ExecuteNonQueryAsync("spLMS_LorryPayments", new { Option = "APPROVE", id, EntryBy = _appUser.CommonEmpNo });
            if (retVal <= 0)
                return Error();

            await EventLog("Logistics - Lorry Payment", "APPROVE", product, shipmentId, Amount, TotalAmount);
            return Message("Payment was approved.");
        }

        [HttpPost]
        [ValidateAction(Forms.Logistics.LorryPayment, Rights.Approve)]
        public async Task<ReturnMessage> UndoLorryPayment([FromForm] long id, [FromForm] string product, [FromForm] string shipmentId, [FromForm] decimal Amount, [FromForm] decimal TotalAmount)
        {
            var errors = await _context.ValidateAsync("spLMS_LorryPayments", new { Option = "VAL_REJECT", id });
            if (errors.Any())
                return ApproveError(errors);

            int retVal = await _context.ExecuteNonQueryAsync("spLMS_LorryPayments", new { Option = "REJECT", id });
            if (retVal <= 0)
                return Error();

            await EventLog("Logistics - Lorry Payment", "REJECT", product, shipmentId, Amount, TotalAmount);
            return Message("Payment was Unapproved.");
        }

        [HttpPost]
        [ValidateAction(Forms.Logistics.LorryPayment, Rights.Modify)]
        public async Task<ReturnMessage> SaveLorryPayment([FromForm] LorryPayment data)
        {
            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "VAL_SAVE"),
                        new SqlParameter("@Product",data.Product),
                        new SqlParameter("@ShipmentID", data.ShipmentID),
                        new SqlParameter("@LorryNumber", data.LorryNumber),
                        new SqlParameter("@StartTime", data.StartTime),
                        new SqlParameter("@EndTime", data.EndTime),
                        new SqlParameter("@Amount", data.Amount),
                        new SqlParameter("@TotalAmount", data.TotalAmount),
                        new SqlParameter("@EntryBy", _appUser.CommonEmpNo),
                        new SqlParameter("@RatePerMins", data.RatePerMins),
                        new SqlParameter("@IsFreeLorryBundle", data.IsFreeLorryBundle),
                        new SqlParameter("@NoOfFreeBundles", data.NoOfFreeBundles),
                        new SqlParameter("@BundleRate", data.BundlesRate),
                        new SqlParameter("@NoOfBundles",  data.NoOfBundles),
                        new SqlParameter("@LorryRate", data.LorryRate),
                        new SqlParameter("@TotalTime", data.TotalTime),
                        new SqlParameter("@ExtraBundleRate", data.ExtraBundleRate),
                        new SqlParameter("@ID", data.ID),
                        new SqlParameter("@NewLorryRate", data.NewLorryRate),
                        new SqlParameter("@VehicleType", data.VehicleType),
                        new SqlParameter("@SubsequentRatePerMins", data.SubsequentRatePerMins),
                        new SqlParameter("@SubsequentNewLorryRate", data.SubsequentNewLorryRate),
                        new SqlParameter("@NewLorryRateFrom", data.NewLorryRateFrom),
                        new SqlParameter("@NewLorryRateTo", data.NewLorryRateTo),
                        new SqlParameter("@FloatSettingID", data.FloatSettingID),
                        new SqlParameter("@BeforeFirstHourRateMinimumMins", data.BeforeFirstHourRateMinimumMins),
                    };
            var lstError = await _context.ValidateSqlAsync("spLMS_LorryPayments", sqlParams);
            if (lstError.Any())
                return SaveError(lstError);

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.AddRange(sqlParams.Select(x => ((ICloneable)x).Clone() as SqlParameter));
            sqlParameters[0].Value = data.ID > 0 ? "UPDATE" : "INSERT";

            var id = await _context.ExecuteSqlNonQueryWithReturnAsync<long>("spLMS_LorryPayments", sqlParameters);

            if (id <= 0)
                return Error();

            await EventLog("Logistics - Lorry Payment", "SAVED", data.Product, data.ShipmentID.ToString(), data.LorryRate, data.BundlesRate, data.Amount, data.TotalAmount);
            return Message("Payment created successfully.");
        }

        [HttpPost]
        public async Task<DataSourceResult> ReadLorryPayment([DataSourceRequest] DataSourceRequest Request, [FromForm] DateTime FromDate, [FromForm] DateTime ToDate, [FromForm] int StatusFilter)
        {
            List<LorryPaymentModel> lorryDetails = await _context.ExecuteSpAsync<LorryPaymentModel>("spLMS_LorryPayments", new { Option = "SELECT_LORRY_LIST", FromDate, ToDate, StatusFilter });
            return lorryDetails.ToDataSourceResult(Request);
        }

        [HttpPost]
        public async Task<List<DropDownModel>> ReadShipment([FromForm] string Product)
        {
            List<DropDownModel> result = await _context.ExecuteSpAsync<DropDownModel>("spLMS_LorryPayments", new { Option = "GET_SHIPMENT_REFNO", Product });
            return result;
        }

        [HttpPost]
        public async Task<List<DropDownModel>> ReadFloat()
        {
            List<DropDownModel> result = await _context.ExecuteSpAsync<DropDownModel>("spLMS_LorryPayments", new { Option = "GET_FLOAT"});
            return result;
        }

        [HttpPost]
        public async Task<List<string>> ReadVehicleType()
        {
            List<StringTypeModel> result = await _context.ExecuteSpAsync<StringTypeModel>("spLMS_LorryPayments", new { Option = "GET_VEHICLE_TYPE" });
            return result.Select(x => x.Value).ToList();
        }

        [HttpPost]
        public async Task<List<string>> ReadProduct()
        {
            List<StringTypeModel> result = await _context.ExecuteSpAsync<StringTypeModel>("spLMS_LorryPayments", new { Option = "GET_PRODUCT" });
            return result.Select(x => x.Value).ToList();
        }

        [HttpPost]
        public async Task<BundleValue> ReadLorryValue([FromForm] string Product)
        {
            return await _context.ExecuteSingleAsync<BundleValue>("spLMS_LorryPayments", new { Option = "GET_BUNDLE_VALUE", Product });

        }

        [HttpPost]
        public async Task<LorryRate> ReadLorryRate([FromForm] string VehicleType)
        {
            return await _context.ExecuteSingleAsync<LorryRate>("spLMS_LorryPayments", new { Option = "GET_LORRY_RATE", VehicleType });

        }

        [HttpGet]
        public async Task<ActionResult> ExportLorryPayment([FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate, [FromQuery] int StatusFilter)
        {
            DataTable dtData = await _context.ExecuteDataTableAsync("spLMS_LorryPayments", new
            {
                Option = "EXPORT_LORRY_LIST",
                FromDate,
                ToDate,
                StatusFilter
            });
            byte[] bytes = new ExcelHelper().Export(dtData, "Lorry Payment", "Lorry Payment Request");
            return File(bytes, MediaTypeNames.Application.Octet, "LorryPayment.xlsx");
        }
        #endregion
        #region Lorry Payment Setting
        [HttpPost]
        [ValidateAction(Forms.Logistics.LorryPaymentSetting, Rights.Modify)]
        public async Task<ReturnMessage> SaveLorryPaymentSetting([FromForm] LorryPaymentSetting data)
        {
            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "VAL_SAVE"),
                        new SqlParameter("@ID",data.ID),
                        new SqlParameter("@LorryType", data.LorryType),
                        new SqlParameter("@NoOfFreeBundles", data.NoOfFreeBundles),
                        new SqlParameter("@ExtraBundleRate", data.ExtraBundleRate),
                        new SqlParameter("@NewLorryRateFrom", data.NewLorryRateFrom),
                        new SqlParameter("@NewLorryRateTo", data.NewLorryRateTo),
                        new SqlParameter("@BeforeFirstHourRate", data.BeforeFirstHourRate),
                        new SqlParameter("@BeforeSubsequentHourRate", data.BeforeSubsequentHourRate),
                        new SqlParameter("@AfterFirstHourRate", data.AfterFirstHourRate),
                        new SqlParameter("@AfterSubsequentHourRate", data.AfterSubsequentHourRate),
                        new SqlParameter("@BeforeFirstHourRateMinimumMins", data.BeforeFirstHourRateMinimumMins),
                        new SqlParameter("@EntryBy", _appUser.CommonEmpNo),
                    };

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.AddRange(sqlParams.Select(x => ((ICloneable)x).Clone() as SqlParameter));
            sqlParameters[0].Value = data.ID > 0 ? "UPDATE" : "INSERT";

            var id = await _context.ExecuteSqlNonQueryWithReturnAsync<long>("spLMS_LorryPaymentSetting", sqlParameters);

            if (id <= 0)
                return Error();

            await EventLog("Logistics - Lorry Payment Setting", "SAVED", data.LorryType, data.NoOfFreeBundles, data.ExtraBundleRate, data.NewLorryRateFrom, data.NewLorryRateTo, data.BeforeFirstHourRate, data.BeforeSubsequentHourRate, data.AfterFirstHourRate, data.AfterSubsequentHourRate);
            return Message("Saved successfully.");
        }

        [HttpPost]
        [ValidateAction(Forms.Logistics.LorryPaymentSetting, Rights.Delete)]
        public async Task<ReturnMessage> DeleteLorryPaymentSetting([FromForm] LorryPaymentSetting data)
        {
            int retVal = await _context.ExecuteNonQueryAsync("spLMS_LorryPaymentSetting", new { Option = "DELETE", data.ID });
            if (retVal <= 0)
                return Error();

            await EventLog("Logistics - Lorry Payment Setting", "SAVED", data.LorryType, data.NoOfFreeBundles, data.ExtraBundleRate, data.NewLorryRateFrom, data.NewLorryRateTo, data.BeforeFirstHourRate, data.BeforeSubsequentHourRate, data.AfterFirstHourRate, data.AfterSubsequentHourRate);
            return Message("Deleted successfully.");
        }

        [HttpPost]
        public async Task<DataSourceResult> ReadLorryPaymentSetting([DataSourceRequest] DataSourceRequest Request)
        {
            List<LorryPaymentSettingModel> lorryDetails = await _context.ExecuteSpAsync<LorryPaymentSettingModel>("spLMS_LorryPaymentSetting", new { Option = "SELECT" });
            return lorryDetails.ToDataSourceResult(Request);
        }
        #endregion
    }
}
