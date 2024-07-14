using BSOL.Core;
using BSOL.Core.Entities;
using BSOL.Core.Models.Accounts;
using BSOL.Core.Models.Common;
using BSOL.Core.Models.HirePurchase;
using BSOL.Core.Models.Sales;
using BSOL.Helpers;
using BSOL.Services;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Data;
using System.Net.Mime;

namespace BSOL.Controllers
{
    public class SalesController : BaseController
    {
        private readonly ICommonHelper _commonHelper;
        private readonly NotificationService _notificationService;
        public SalesController(BSOLContext context, BSOLWebContext webContext, AppUser appUser, ICommonHelper commonHelper, NotificationService notificationService) : base(context, webContext, appUser)
        {
            _commonHelper = commonHelper;
            _notificationService = notificationService;
        }

        #region Spare Parts Request
        [HttpPost]
        public async Task<List<string>> ReadPaymentReferenceNo()
        {
            List<StringTypeModel> result = await _Webcontext.ExecuteSpAsync<StringTypeModel>("spPOS_SparePartsRequest", new { Option = "GET_REMITTANCE" });
            return result.Select(x => x.Value).ToList();
        }
        [HttpPost]
        public async Task<CustomDetailModel> ReadCustomerDetails([FromForm] long CustomerId)
        {
            return await _Webcontext.ExecuteSingleAsync<CustomDetailModel>("spPOS_SparePartsRequest", new { Option = "GET_CUSTOMER_DETAILS", CustomerID = CustomerId });
        }
        [HttpPost]
        public async Task<List<string>> ReadVesselName()
        {
            List<StringTypeModel> result = await _Webcontext.ExecuteSpAsync<StringTypeModel>("spPOS_SparePartsRequest", new { Option = "GET_VESSEL_NAME" });
            return result.Select(x => x.Value).ToList();
        }
        [HttpPost]
        public async Task<StringTypeModel> ReadVesselContactNo([FromForm] string VesselName)
        {
            return await _Webcontext.ExecuteSingleAsync<StringTypeModel>("spPOS_SparePartsRequest", new { Option = "GET_VESSEL_DETAILS", VesselName });
        }
        [HttpPost]
        public async Task<DateTimeTypeModel> ReadRemittanceDetails([FromForm] string PaymentReferenceNo)
        {
            return await _Webcontext.ExecuteSingleAsync<DateTimeTypeModel>("spPOS_SparePartsRequest", new { Option = "GET_REMITTANCE_DETAILS", PaymentReferenceNo });
        }
        [HttpPost]
        public async Task<List<string>> ReadDeliveryAddress()
        {
            return await _Webcontext.SparePartsRequests.Where(x => x.DeliveryAddress != null).Select(x => x.DeliveryAddress).Distinct().ToListAsync();
        }
        [HttpPost]
        [ValidateAction(Forms.Sales.SparePartsRequest, Rights.View)]
        public async Task<DataSourceResult> ReadSparePartsRequest([DataSourceRequest] DataSourceRequest request, [FromForm] int StatusFilter, [FromForm] DateTime FromDate, [FromForm] DateTime ToDate)
        {
            var result = await _Webcontext.ExecuteSpAsync<SparePartsRequestModel>("spPOS_SparePartsRequest", new { Option = "SELECT", StatusFilter, FromDate, ToDate });
            return await result.ToDataSourceResultAsync(request);
        }

        [HttpPost]
        [ValidateAction(Forms.Sales.SparePartsRequest, Rights.Modify)]
        public async Task<ReturnMessage> SaveSparePartsRequest([FromForm] SparePartsRequest datas)
        {
            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "VAL_SAVE"),
                        new SqlParameter("@ID", datas.ID),
                        new SqlParameter("@QuotationNo", datas.QuotationNo),
                        new SqlParameter("@Product", datas.Product),
                        new SqlParameter("@CustomerName", datas.CustomerName),
                        new SqlParameter("@ContactNo", datas.ContactNo),
                        new SqlParameter("@DeliveryAddress", datas.DeliveryAddress),
                        new SqlParameter("@DeliverTo", datas.DeliverTo),
                        new SqlParameter("@CustomerAddress", datas.CustomerAddress),
                        new SqlParameter("@RequiredDeliveryOn", datas.RequiredDeliveryOn),
                        new SqlParameter("@VesselName", datas.VesselName),
                        new SqlParameter("@VesselContact", datas.VesselContact),
                        new SqlParameter("@RequestReceivedBy", datas.RequestReceivedBy),
                        new SqlParameter("@RequestReceivedOn", datas.RequestReceivedOn),
                        new SqlParameter("@RequestReceivedTo", datas.RequestReceivedTo),
                        new SqlParameter("@Remarks", datas.Remarks),
                        new SqlParameter("@EntryBy", _appUser.CommonEmpNo),

                        new SqlParameter("@PaymentReferenceNo", datas.PaymentReferenceNo),
                        new SqlParameter("@PaymentReceivedOn", datas.PaymentReceivedOn),
                        new SqlParameter("@BillRefNo", datas.BillRefNo),
                        new SqlParameter("@BillAmount", datas.BillAmount),
                        new SqlParameter("@Currency", datas.Currency),

                        new SqlParameter("@CustomerID", datas.CustomerID),
                        new SqlParameter("@IslandID", datas.IslandID),
                        new SqlParameter("@ActualDeliveryAddress", datas.ActualDeliveryAddress),
                        new SqlParameter("@ActualDeliveryTo", datas.ActualDeliveryTo),
                        new SqlParameter("@AssignedTo", datas.AssignedTo),
                        new SqlParameter("@AssignedDate", datas.AssignedDate),
                        new SqlParameter("@DeliveryBy", datas.DeliveryBy),
                        new SqlParameter("@DeliveryDate", datas.DeliveryDate),
                        new SqlParameter("@AssignedToId", datas.AssignedToId),
                        new SqlParameter("@DeliveryById", datas.DeliveryById),
                        new SqlParameter("@IslandCode", datas.IslandCode)
                    };

            var errors = await _Webcontext.ValidateSqlAsync("spPOS_SparePartsRequest", sqlParams);
            if (errors.Any())
                return ApproveError(errors);

            List<SqlParameter> sqlParameter = new List<SqlParameter>();
            sqlParameter.AddRange(sqlParams.Select(x => ((ICloneable)x).Clone() as SqlParameter));
            sqlParameter[0].Value = datas.ID == 0 ? "INSERT" : "UPDATE";

            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("spPOS_SparePartsRequest", sqlParameter);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("Sales - SparePartsRequest", "UPDATE", datas.QuotationNo, datas.PaymentReferenceNo);
            return Message("Saved successfully.");
        }
        [HttpPost]
        [ValidateAction(Forms.Sales.SparePartsRequest, Rights.Modify)]
        public async Task<ReturnMessage> UpdateBilling([FromForm] SparePartsRequest datas)
        {

            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "UPDATE_BILL_LIST"),
                        new SqlParameter("@ID", datas.ID),
                        new SqlParameter("@QuotationNo", datas.QuotationNo),
                        new SqlParameter("@Product", datas.Product),
                        new SqlParameter("@CustomerName", datas.CustomerName),
                        new SqlParameter("@ContactNo", datas.ContactNo),
                        new SqlParameter("@DeliveryAddress", datas.DeliveryAddress),
                        new SqlParameter("@DeliverTo", datas.DeliverTo),
                        new SqlParameter("@CustomerAddress", datas.CustomerAddress),
                        new SqlParameter("@RequiredDeliveryOn", datas.RequiredDeliveryOn),
                        new SqlParameter("@VesselName", datas.VesselName),
                        new SqlParameter("@VesselContact", datas.VesselContact),
                        new SqlParameter("@RequestReceivedBy", datas.RequestReceivedBy),
                        new SqlParameter("@RequestReceivedOn", datas.RequestReceivedOn),
                        new SqlParameter("@RequestReceivedTo", datas.RequestReceivedTo),
                        new SqlParameter("@Remarks", datas.Remarks),
                        new SqlParameter("@EntryBy", _appUser.CommonEmpNo),

                        new SqlParameter("@PaymentReferenceNo", datas.PaymentReferenceNo),
                        new SqlParameter("@PaymentReceivedOn", datas.PaymentReceivedOn),
                        new SqlParameter("@BillRefNo", datas.BillRefNo),
                        new SqlParameter("@BillAmount", datas.BillAmount),
                        new SqlParameter("@Currency", datas.Currency),

                        new SqlParameter("@CustomerID", datas.CustomerID),
                        new SqlParameter("@IslandID", datas.IslandID),
                        new SqlParameter("@ActualDeliveryAddress", datas.ActualDeliveryAddress),
                        new SqlParameter("@ActualDeliveryTo", datas.ActualDeliveryTo),
                        new SqlParameter("@AssignedTo", datas.AssignedTo),
                        new SqlParameter("@AssignedDate", datas.AssignedDate),
                        new SqlParameter("@DeliveryBy", datas.DeliveryBy),
                        new SqlParameter("@DeliveryDate", datas.DeliveryDate),
                        new SqlParameter("@AssignedToId", datas.AssignedToId),
                        new SqlParameter("@DeliveryById", datas.DeliveryById),
                        new SqlParameter("@IslandCode", datas.IslandCode)
                    };

            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("spPOS_SparePartsRequest", sqlParams);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("Sales - SparePartsRequest", "UPDATE_BILL", datas.QuotationNo, datas.PaymentReferenceNo);
            return Message("Updated successfully.");
        }
        [HttpPost]
        [ValidateAction(Forms.Sales.SparePartsRequest, Rights.Modify)]
        public async Task<ReturnMessage> UpdatePacking([FromForm] List<SparePartsRequest> datas)
        {
            var tableSchemas = new List<SqlMetaData>() {
                  new SqlMetaData("Number", SqlDbType.BigInt)
            }.ToArray();

            var sqlDataRecords = new List<SqlDataRecord>();
            datas.ForEach(x =>
            {
                SqlDataRecord sqlDataRecord = new SqlDataRecord(tableSchemas);
                sqlDataRecord.SetInt64(0, (long)x.ID);
                sqlDataRecords.Add(sqlDataRecord);
            });

            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "UPDATE_PACKING"),
                        new SqlParameter("@EntryBy", _appUser.CommonEmpNo),
                        new SqlParameter
                        {
                            ParameterName = "@Number",
                            Value = sqlDataRecords,
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "dbo.NumberType"
                        }
                    };

            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("spPOS_SparePartsRequest", sqlParams);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            var billinglst = string.Join('/', datas.Select(x => x.BillRefNo));
            await EventLog("Sales - SparePartsRequest", "PACKING", billinglst);
            return Message("Saved successfully.");
        }
        [HttpPost]
        [ValidateAction(Forms.Sales.SparePartsRequest, Rights.Modify)]
        public async Task<ReturnMessage> UpdateDelivery([FromForm] SparePartsRequest datas)
        {

            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "UPDATE_DELIVERY"),
                        new SqlParameter("@ID", datas.ID),
                        new SqlParameter("@QuotationNo", datas.QuotationNo),
                        new SqlParameter("@Product", datas.Product),
                        new SqlParameter("@CustomerName", datas.CustomerName),
                        new SqlParameter("@ContactNo", datas.ContactNo),
                        new SqlParameter("@DeliveryAddress", datas.DeliveryAddress),
                        new SqlParameter("@DeliverTo", datas.DeliverTo),
                        new SqlParameter("@CustomerAddress", datas.CustomerAddress),
                        new SqlParameter("@RequiredDeliveryOn", datas.RequiredDeliveryOn),
                        new SqlParameter("@VesselName", datas.VesselName),
                        new SqlParameter("@VesselContact", datas.VesselContact),
                        new SqlParameter("@RequestReceivedBy", datas.RequestReceivedBy),
                        new SqlParameter("@RequestReceivedOn", datas.RequestReceivedOn),
                        new SqlParameter("@RequestReceivedTo", datas.RequestReceivedTo),
                        new SqlParameter("@Remarks", datas.Remarks),
                        new SqlParameter("@EntryBy", _appUser.CommonEmpNo),

                        new SqlParameter("@PaymentReferenceNo", datas.PaymentReferenceNo),
                        new SqlParameter("@PaymentReceivedOn", datas.PaymentReceivedOn),
                        new SqlParameter("@BillRefNo", datas.BillRefNo),
                        new SqlParameter("@BillAmount", datas.BillAmount),
                        new SqlParameter("@Currency", datas.Currency),

                        new SqlParameter("@CustomerID", datas.CustomerID),
                        new SqlParameter("@IslandID", datas.IslandID),
                        new SqlParameter("@ActualDeliveryAddress", datas.ActualDeliveryAddress),
                        new SqlParameter("@ActualDeliveryTo", datas.ActualDeliveryTo),
                        new SqlParameter("@AssignedTo", datas.AssignedTo),
                        new SqlParameter("@AssignedDate", datas.AssignedDate),
                        new SqlParameter("@DeliveryBy", datas.DeliveryBy),
                        new SqlParameter("@DeliveryDate", datas.DeliveryDate),
                        new SqlParameter("@AssignedToId", datas.AssignedToId),
                        new SqlParameter("@DeliveryById", datas.DeliveryById),
                        new SqlParameter("@IslandCode", datas.IslandCode),
                         new SqlParameter("@AcknowledgedRemarks", datas.AcknowledgedRemarks)
                    };

            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("spPOS_SparePartsRequest", sqlParams);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("Sales - SparePartsRequest", "UPDATE_DELIVERY", datas.BillRefNo);

            if (retVal > 0 &&  datas.DeliveryById == null && datas.AssignedToId > 0)
            {
                _notificationService.Send(Helpers.Notification.POSSparePartsRequest.JobAssigned, datas.ID);
            }

            if (retVal > 0 && datas.DeliveryById > 0)
            {
                _notificationService.Send(Helpers.Notification.POSSparePartsRequest.JobDelivered, datas.ID);
            }
            return Message("Updated successfully.");
        }
        [HttpPost]
        [ValidateAction(Forms.Sales.SparePartsRequest, Rights.Modify)]
        public async Task<ReturnMessage> UpdateAcknowledge([FromForm] SparePartsRequest datas)
        {

            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "UPDATE_ACKNOWLEDGE"),
                        new SqlParameter("@ID", datas.ID),
                        new SqlParameter("@AcknowledgedRemarks", datas.AcknowledgedRemarks),
                        new SqlParameter("@EntryBy", _appUser.CommonEmpNo)
                    };

            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("spPOS_SparePartsRequest", sqlParams);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("Sales - SparePartsRequest", "UPDATE_ACKNOWLEDGE", datas.BillRefNo);
            return Message("Updated successfully.");
        }
        [HttpPost]
        [ValidateAction(Forms.Sales.SparePartsRequest, Rights.Delete)]
        public async Task<ReturnMessage> DeleteSparePartsRequest([FromForm] long ID, [FromForm] string QuoataionNo, [FromForm] string ReasonForCancel)
        {

            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "VAL_DELETE"),
                        new SqlParameter("@ID", ID),
                        new SqlParameter("@ReasonForCancel", ReasonForCancel)
                    };

            var errors = await _Webcontext.ValidateSqlAsync("spPOS_SparePartsRequest", sqlParams);
            if (errors.Any())
                return DeleteError(errors);

            List<SqlParameter> sqlParameter = new List<SqlParameter>();
            sqlParameter.AddRange(sqlParams.Select(x => ((ICloneable)x).Clone() as SqlParameter));
            sqlParameter[0].Value = "DELETE";


            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("spPOS_SparePartsRequest", sqlParameter);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("Sales - SparePartsRequest", "DELETE", QuoataionNo);
            return Message("Deleted successfully.");
        }
        [HttpPost]
        [ValidateAction(Forms.Sales.SparePartsRequest, Rights.Approve)]
        public async Task<ReturnMessage> ApprovedAcknowledgedProcess([FromForm] SparePartsRequest datas)
        {

            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "APPROVE"),
                        new SqlParameter("@ID", datas.ID),
                        new SqlParameter("@EntryBy", _appUser.CommonEmpNo)
                    };


            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("spPOS_SparePartsRequest", sqlParams);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("Sales - SparePartsRequest", "APPROVE", datas.QuotationNo, datas.BillRefNo);
            return Message("Approved successfully.");
        }
        [HttpPost]
        [ValidateAction(Forms.Sales.SparePartsRequest, Rights.Approve)]
        public async Task<ReturnMessage> RevertAcknowledgedProcess([FromForm] SparePartsRequest datas)
        {

            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "UNDO"),
                        new SqlParameter("@ID", datas.ID)
                    };

            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("spPOS_SparePartsRequest", sqlParams);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("Sales - SparePartsRequest", "UNDO", datas.QuotationNo, datas.BillRefNo);
            return Message("Undo successfully.");
        }
        [HttpPost]
        [ValidateAction(Forms.Sales.SparePartsRequest, Rights.Modify)]
        public async Task<ReturnMessage> RevertSparePartsRequest([FromForm] SparePartsRequest datas, [FromForm] int StatusFilter, [FromForm] string ReasonForEdit = "")
        {
            var option = "";
            var valOption = "";
            option = StatusFilter == 1 ? "UNDO_PAYMENT_PROCESS" : StatusFilter == 2 ? "UNDO_BILL_LIST" : StatusFilter == 3 ? "UNDO_PACKING" : StatusFilter == 4 ? "UNDO_DELIVERY" : "UNDO_ACKNOWLEDGE";
            valOption = StatusFilter == 1 ? "VAL_UNDO_PAYMENT_PROCESS" : StatusFilter == 2 ? "VAL_UNDO_BILL_LIST" : StatusFilter == 3 ? "VAL_UNDO_PACKING" : StatusFilter == 4 ? "VAL_UNDO_DELIVERY" : "VAL_UNDO_ACKNOWLEDGE";

            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", valOption),
                        new SqlParameter("@ID", datas.ID),
                        new SqlParameter("@ReasonForEdit", ReasonForEdit)
                    };

            var errors = await _Webcontext.ValidateSqlAsync("spPOS_SparePartsRequest", sqlParams);
            if (errors.Any())
                return UndoError(errors);

            List<SqlParameter> sqlParameter = new List<SqlParameter>();
            sqlParameter.AddRange(sqlParams.Select(x => ((ICloneable)x).Clone() as SqlParameter));
            sqlParameter[0].Value = option;

            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("spPOS_SparePartsRequest", sqlParameter);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            if (option == "UNDO_PACKING")
                await new Document() { DocumentRoot = _commonHelper.GetDocumentRoot() }.DeleteByReference(DocumentReference.SpareParts, datas.ID.ToString());

            await EventLog("Sales - SparePartsRequest", "UNDO", datas.QuotationNo, datas.BillRefNo);
            return Message("Reverted successfully.");
        }
        #endregion
        #region Call Log Details
        [HttpPost]
        [ValidateAction(Forms.Sales.SparePartsRequest, Rights.View)]
        public async Task<DataSourceResult> ReadCallLogs([DataSourceRequest] DataSourceRequest request, [FromForm] long RequestID)
        {
            return await _Webcontext.CallLogs.OrderBy(x => x.CallDate).Where(x => x.ReferenceId == RequestID && x.Module == "SPARE_PARTS").ToDataSourceResultAsync(request);
        }

        [HttpPost]
        [ValidateAction(Forms.Sales.SparePartsRequest, Rights.Modify)]
        public async Task<ReturnMessage> UpdateCallLog([FromForm] CallLog datas)
        {
            datas.Module = "SPARE_PARTS";
            if (!await datas.SaveAsync())
                SaveError(datas.ErrorMessage);

            return Message("Saved successfully");
        }

        [HttpPost]
        [ValidateAction(Forms.Sales.SparePartsRequest, Rights.Delete)]
        public async Task<ReturnMessage> DeleteCallLog([FromForm] CallLog data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Call log deleted.");
        }
        [HttpPost]
        public async Task<List<string>> ReadSubject()
        {
            var query1 = await (from cl in _Webcontext.CallLogs
                                where cl.Subject != "" && cl.Subject != "Delivery Confirmation" && cl.Subject != "Delivery Status" && cl.Subject != "Others"
                                select cl.Subject).Distinct().ToListAsync();

            string[] defaultvaue = new string[] { "Delivery Confirmation", "Delivery Status", "Others" };

            query1.AddRange(defaultvaue);
            return query1;
        }
        #endregion
        #region Spare Parts Delivery
        [HttpPost]
        public async Task<List<SparePartsDeliveryStausModel>> GetPendingDelivery()
        {
            List<SparePartsDeliveryStausModel> result = await _Webcontext.ExecuteSpAsync<SparePartsDeliveryStausModel>("spPOS_SparePartsDeliveryStatus", new { Option = "UN_DELIVERY_LIST", AppUser = _appUser.EmployeeId });
            return result;
        }
        [HttpPost]
        public async Task<ReturnMessage> UpdateDeliveryStatus([FromForm] DateTime DeliveryDate, [FromForm] string DRN, [FromForm] long ID)
        {
            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "UPDATE_DELIVERY"),
                        new SqlParameter("@ID", ID),
                        new SqlParameter("@AppUser", _appUser.EmployeeId),
                        new SqlParameter("@EntryBy", _appUser.CommonEmpNo),
                        new SqlParameter("@DeliveryDate", DeliveryDate),
                    };

            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("spPOS_SparePartsDeliveryStatus", sqlParams);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("Sales - SparePartsRequest", "DELIVERY", DRN, DeliveryDate);
            _notificationService.Send(Helpers.Notification.POSSparePartsRequest.JobDelivered, ID);
            return Message("Updated successfully.");
        }
        [HttpPost]
        public async Task<IntegerTypeModel> GetNotificationCount()
        {
            return await _Webcontext.ExecuteSingleAsync<IntegerTypeModel>("spPOS_SparePartsUnDeliveryNotification", new { AppUser = _appUser.EmployeeId });
        }
        [HttpGet]
        public async Task<ActionResult> ExportSparePartsRequest([FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate, [FromQuery] int StatusFilter)
        {
            DataTable dtData = await _Webcontext.ExecuteDataTableAsync("spPOS_SparePartsRequest", new
            {
                Option = "EXPORT_SPARE_PARTS_LIST",
                FromDate,
                ToDate,
                StatusFilter
            });
            byte[] bytes = new ExcelHelper().Export(dtData, "Spare Parts Request", "Spare Parts Request");
            return File(bytes, MediaTypeNames.Application.Octet, "SparePartsRequest.xlsx");
        }
        #endregion

        #region Sale Summary
        public async Task<FileContentResult> ExportSalesSummary([FromForm] DateTime? fromDate = null)
        {
            DataTable dataTable;
            dataTable = await _Webcontext.ExecuteDataTableAsync("spPOS_ItemSummary", new { FDate = fromDate });

            byte[] bytes = new ExcelHelper().Export(dataTable, "Sales Summary List");
            return File(bytes, MediaTypeNames.Application.Octet, "SalesSummary.xlsx");
        }
        #endregion 
    }
}
