using BSOL.Core;
using BSOL.Core.Entities;
using BSOL.Core.Models.Accounts;
using BSOL.Core.Models.Common;
using BSOL.Helpers;
using BSOL.Services;
using DocumentFormat.OpenXml.VariantTypes;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using DocumentFormat.OpenXml.Vml.Office;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace BSOL.Controllers
{
    public class PaymentMasterController : BaseController
    {
        private readonly ICommonHelper _commonHelper;
        public PaymentMasterController(BSOLContext context, BSOLWebContext webContext, AppUser appUser, ICommonHelper commonHelper) : base(context, webContext, appUser)
        {
            _commonHelper = commonHelper;
        }
        [HttpPost]
        public async Task<List<SupplierBankAccountTypeModel>> ReadBank([FromForm] long SupplierId)
        {
            //return await (from b in _Webcontext.Banks
            //              join sb in _Webcontext.SupplierBankDetails on b.ID equals sb.BankId
            //              where sb.SupplierId == SupplierId
            //              select new SupplierBankAccountTypeModel
            //              {
            //                  Id = b.ID,
            //                  Text = b.BankName,
            //                  IsPrimary = sb.IsPrimary,
            //                  AccountName = sb.AccountName,
            //                  AccountNo = sb.AccountNo
            //              })
            //  .Distinct()
            //  .ToListAsync();

            var result = (from b in _Webcontext.Banks
                                join sb in _Webcontext.SupplierBankDetails on b.ID equals sb.BankId
                                where sb.SupplierId == SupplierId
                                select new
                                {
                                    Id = b.ID,
                                    BankName = b.BankName,
                                    IsPrimary = sb.IsPrimary,
                                    AccountName = sb.AccountName,
                                    AccountNo = sb.AccountNo
                                })
                       .AsEnumerable()  // Switch to client-side evaluation
                       .GroupBy(x => x.BankName)
                       .Select(g => g.FirstOrDefault())
                       .Select(x => new SupplierBankAccountTypeModel
                       {
                           Id = x.Id,
                           Text = x.BankName,
                           IsPrimary = x.IsPrimary,
                           AccountName = x.AccountName,
                           AccountNo = x.AccountNo
                       })
                       .ToList();

            return result;
        }
        [HttpPost]
        public async Task<List<SupplierBankAccountTypeModel>> ReadSupplierAccountList([FromForm] long SupplierId, [FromForm] long BankId, [FromForm] long AccountNo)
        {
            return await (from sb in _Webcontext.SupplierBankDetails
                          join b in _Webcontext.Banks on sb.BankId equals b.ID
                          where sb.SupplierId == SupplierId && sb.BankId == BankId && sb.AccountNo == AccountNo
                          select new SupplierBankAccountTypeModel
                          {
                              Text = sb.AccountName,
                              Id = sb.AccountNo,
                              IsPrimary = sb.IsPrimary
                          }).DistinctBy(x => x.Id)
                            .ToListAsync();
        }
        [HttpPost]
        public async Task<List<SupplierBankAccountTypeModel>> ReadSupplierAccountNoList([FromForm] long SupplierId, [FromForm] long BankId)
        {
            return await (from sb in _Webcontext.SupplierBankDetails
                          join b in _Webcontext.Banks on sb.BankId equals b.ID
                          where sb.SupplierId == SupplierId && sb.BankId == BankId /*&& sb.AccountName == accountName*/
                          select new SupplierBankAccountTypeModel
                          {
                              Text = sb.AccountName,
                              Id = sb.AccountNo,
                              IsPrimary = sb.IsPrimary
                          }).ToListAsync();
        }
        public async Task<List<DecimalTypeModel>> ReadGSTPercent()
        {
            return await _Webcontext.PaymentDetails.Where(x => x.GSTPercent > 0).Select(x => new DecimalTypeModel { Value = x.GSTPercent ?? 0 }).Distinct().ToListAsync();
        }
        public async Task<List<DropDownModel>> ReadShipments([FromForm] long SupplierId)
        {
            return await (from sh in _Webcontext.Shipments
                          where sh.CompanyId == _appUser.CompanyId && sh.Active && sh.ClearedDate.HasValue
                          && (from spd in _Webcontext.ShipmentPurchaseOrderDetails
                              join pod in _Webcontext.PurchaseOrderDetails on spd.PurchaseOrderDetailId equals pod.Id
                              join pd in _Webcontext.PurchaseOrders on pod.PurchaseOrderId equals pd.Id
                              where pd.SupplierId == SupplierId && sh.Id == spd.ShipmentId
                              select pd.Id).Any()
                          select new DropDownModel { Id = sh.Id, Text = sh.RefNoFormatted }).ToListAsync();
        }
        public async Task<List<DropDownModel>> ReadWithoutPO([FromForm] long SupplierId)
        {
            return await (from pm in _Webcontext.PaymentMasters
                          where pm.SupplierId == SupplierId && pm.Active && pm.PaymentCategory == "Without PO Invoice"
                          && (from spd in _Webcontext.PaymentMasters
                              where spd.SupplierId == SupplierId && pm.ID != spd.ParentId
                              select spd.ID).Any()
                          select new DropDownModel { Id = pm.ID, Text = pm.RefNoFormatted }).ToListAsync();
        }
        public async Task<DataSourceResult> ReadShipmentExpenses([DataSourceRequest] DataSourceRequest request, [FromForm] long shipmentId, [FromForm] long ID)
        {
            var result = await _Webcontext.ExecuteSpAsync<PaymentDetails>("spFMS_PaymentRequest", new { Option = "GET_SHIPMENT_EXPENSE", _appUser.CompanyId, ShipmentId = shipmentId, ID });
            return await result.ToDataSourceResultAsync(request);
        }
        public async Task<DataSourceResult> ReadPurchaseOrders([DataSourceRequest] DataSourceRequest request, [FromForm] long supplierId = 0, [FromForm] string currency = "", [FromForm] string payeeType = "", [FromForm] string SupplierName = "", [FromForm] long BusinessUnitId = 0, [FromForm] string paymentMode = null)
        {
            var result = await _Webcontext.ExecuteSpAsync<SupplierPurchaseOrderModel>("spFMS_PaymentRequest", new { Option = "GET_PURCHASE_ORDER", _appUser.CompanyId, SupplierId = supplierId, Currency = currency, payeeType , SupplierName, BusinessUnitId, paymentMode });
            return await result.ToDataSourceResultAsync(request);
        }
        public async Task<DataSourceResult> ReadSupplierInvoiceDetails([DataSourceRequest] DataSourceRequest request, [FromForm] long ID = 0, [FromForm] string paymentMode = null)
        {
            var result = await _Webcontext.ExecuteSpAsync<SupplierPurchaseOrderModel>("spFMS_PaymentRequest", new { Option = "SELECT_PURCHASE_ORDER", _appUser.CompanyId, ID , paymentMode });
            return await result.ToDataSourceResultAsync(request);
        }
        public async Task<DataSourceResult> ReadWOPSupplierInvoices([DataSourceRequest] DataSourceRequest request, [FromForm] long ID = 0, [FromForm] string PayeeType = "", [FromForm] long BusinessUnitId = 0, [FromForm] string PaymentCategory = "")
        {
            var result = await _Webcontext.ExecuteSpAsync<PaymentDetails>("spFMS_PaymentRequest", new { Option = "GET_WOPURCHASE_ORDER", _appUser.CompanyId, ID, PayeeType, BusinessUnitId, PaymentCategory });
            return await result.ToDataSourceResultAsync(request);
        }
        [ValidateAction(Forms.Accounts.PaymentRequest, Rights.View)]
        public async Task<DataSourceResult> ReadSupplierInvoices([DataSourceRequest] DataSourceRequest request, [FromForm] int statusFilter = 0)
        {
            var result = await _Webcontext.ExecuteSpAsync<PaymentMasterModel>("spFMS_PaymentRequest", new { Option = "SELECT", _appUser.CompanyId, StatusFilter = statusFilter });
            return await result.ToDataSourceResultAsync(request);
        }
        [ValidateAction(Forms.Accounts.PaymentRequest, Rights.View)]
        public async Task<PaymentMasterModel> GetSupplierInvoices([FromForm] long ID = 0)
        {
            return await _Webcontext.ExecuteSingleAsync<PaymentMasterModel>("spFMS_PaymentRequest", new { Option = "SELECT", _appUser.CompanyId, ID });
        }
        public async Task<List<SupplierPurchaseOrderModel>> GetParentInvoiceList([FromForm] long ID = 0)
        {
            return await _Webcontext.ExecuteSpAsync<SupplierPurchaseOrderModel>("spFMS_PaymentRequest", new { Option = "SELECT_PURCHASE_ORDER", _appUser.CompanyId, ID });
        }

        [ValidateAction(Forms.Accounts.PaymentRequest, Rights.Modify)]
        public async Task<ReturnMessage> SavePaymentRequest([FromForm] PaymentRequest datas, [FromForm] List<PaymentDetails> paymentDetails)
        {
            var tableSchema = new List<SqlMetaData>() {
                new SqlMetaData("ID", SqlDbType.BigInt),
                new SqlMetaData("ReferenceId", SqlDbType.BigInt),
                new SqlMetaData("Currency", SqlDbType.VarChar, -1),
                new SqlMetaData("GSTAmount", SqlDbType.Decimal,18,2),
                new SqlMetaData("Amount", SqlDbType.Decimal,18,2),
                new SqlMetaData("GSTPercent", SqlDbType.Decimal,18,2),
                new SqlMetaData("ExpenseCode", SqlDbType.VarChar, -1),
                new SqlMetaData("Details", SqlDbType.VarChar, -1),
                new SqlMetaData("ReferenceNo", SqlDbType.VarChar, -1),
                new SqlMetaData("MaxAmount", SqlDbType.Decimal,18,2),
                new SqlMetaData("GSTSettingId", SqlDbType.BigInt)
            }.ToArray();
            var sqlDataRecords = new List<SqlDataRecord>();
            paymentDetails.ForEach(x =>
            {
                SqlDataRecord sqlDataRecord = new SqlDataRecord(tableSchema);
                sqlDataRecord.SetInt64(0, x.ID);
                sqlDataRecord.SetInt64(1, x.ReferenceId ?? 0);
                sqlDataRecord.SetString(2, datas.Currency);
                sqlDataRecord.SetDecimal(3, (decimal)x.GSTAmount);
                sqlDataRecord.SetDecimal(4, (decimal)x.Amount);
                sqlDataRecord.SetDecimal(5, (decimal)x.GSTPercent);
                sqlDataRecord.SetString(6, x.ExpenseCode);
                sqlDataRecord.SetString(7, x.Details ?? "");
                sqlDataRecord.SetString(8, x.ReferenceNo ?? "");
                sqlDataRecord.SetDecimal(9, (decimal)x.MaxAmount);
                sqlDataRecord.SetInt64(10, x.GSTSettingId ?? 0);
                sqlDataRecords.Add(sqlDataRecord);
            });

            List<SqlParameter> sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@Option", "VAL_SAVE"),
                new SqlParameter("@ID", datas.ID),
                new SqlParameter("@InvoiceNo", datas.InvoiceNo),
                new SqlParameter("@InvoiceDate", datas.InvoiceDate?? null),
                new SqlParameter("@ReferenceId", datas.ReferenceId),
                new SqlParameter("@ReferenceNo", datas.ReferenceNo),
                new SqlParameter("@BusinessUnitId", datas.BusinessUnitId),
                new SqlParameter("@SupplierId", datas.SupplierId),
                new SqlParameter("@PayeeType", datas.PayeeName),
                new SqlParameter("@PaymentCategory", datas.Category),
                new SqlParameter("@Currency", datas.Currency),
                new SqlParameter("@Amount", datas.Amount),
                new SqlParameter("@GST", datas.GST),
                new SqlParameter("@TotalAmount", datas.TotalAmount),
                new SqlParameter("@GSTPercent", datas.GSTPercent),
                new SqlParameter("@PaymentMode", datas.PaymentType),
                new SqlParameter("@BankId", datas.BankId),
                new SqlParameter("@BankName", datas.BankName),
                new SqlParameter("@AccountName", datas.AccountName),
                new SqlParameter("@AccountNo", datas.AccountNo),
                new SqlParameter("@PayeeName", datas.PayeeName),
                new SqlParameter("@DebitTo", datas.DebitTo),
                new SqlParameter("@CreditTo", datas.CreditTo),
                new SqlParameter("@EntryBy", _appUser.CommonEmpNo),
                new SqlParameter("@Remarks", datas.Remarks),
                new SqlParameter("@CompanyId", _appUser.CompanyId),
                new SqlParameter("@ParentId", datas.ParentId),
                new SqlParameter("@EmployeeId", _appUser.EmployeeId),
                new SqlParameter("@EntityID", _appUser.EntityID),
                new SqlParameter
                {
                    ParameterName = "@PaymentDetailsType",
                    Value = sqlDataRecords,
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.PaymentDetailsType"
                }
            };

            var errors = await _Webcontext.ValidateSqlAsync("spFMS_PaymentRequest", sqlParams);
            if (errors.Any())
                return SaveError(errors);
            sqlParams[0].Value = datas.ID > 0 ? "UPDATE" : "INSERT";

            long Id = await _Webcontext.ExecuteSqlNonQueryWithReturnAsync<long>("spFMS_PaymentRequest", sqlParams);
            if (Id <= 0)
                return Error(Constants.DBErrorMessage);

            if (Id > 0)
                await EventLog("PAYMENT REQUEST", ActionType.Edit.ToString(), "PaymentRequest", datas.ID, datas.InvoiceNo);

            return Message("Payment Request saved.", Id);
        }

        [ValidateAction(Forms.Accounts.PaymentRequest, Rights.Delete)]
        public async Task<ReturnMessage> DeletePaymentMaster([FromForm] long ID, [FromForm] string InvoiceNo)
        {
            var errors = await _Webcontext.ValidateAsync("spFMS_PaymentRequest", new { Option = "VAL_DELETE", ID });
            if (errors.Any())
                return DeleteError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spFMS_PaymentRequest", new { Option = "DELETE", ID });
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await new Document() { DocumentRoot = _commonHelper.GetDocumentRoot() }.DeleteByReference(DocumentReference.SupplierPayment, ID.ToString());

            await EventLog("PAYMENT REQUEST", ActionType.Delete.ToString(), "PaymentRequest", ID, InvoiceNo);
            return Message("Payment request deleted.");
        }
        [ValidateAction(Forms.Accounts.PaymentRequest, Rights.Delete)]
        public async Task<ReturnMessage> DeletePaymentInvoiceDetails([FromForm] long ID, [FromForm] long PaymentDetailsID, [FromForm] string RefNo)
        {
            var errors = await _Webcontext.ValidateAsync("spFMS_PaymentRequest", new { Option = "VAL_DELETE_DETAILS", ID, PaymentDetailsID });
            if (errors.Any())
                return DeleteError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spFMS_PaymentRequest", new { Option = "DELETE_DETAILS", ID, PaymentDetailsID });
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("PAYMENT REQUEST DETAILS", ActionType.Delete.ToString(), "PaymentRequestDetails", ID, RefNo);
            return Message("Payment request details deleted.");
        }
        [ValidateAction(Forms.Accounts.PaymentRequest, Rights.Approve)]
        public async Task<ReturnMessage> ApproveSupplierInvoice([FromForm] long ID, [FromForm] string InvoiceNo)
        {
            int retVal = await _Webcontext.ExecuteNonQueryAsync("spFMS_PaymentRequest", new { Option = "APPROVE", ID, EntryBy = _appUser.CommonEmpNo });
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("SUPPLIER INVOICE", ActionType.Approve.ToString(), "PaymentRequestDetails", ID);
            return Message("Approved successfully.");
        }
        [ValidateAction(Forms.Accounts.PaymentRequest, Rights.Approve)]
        public async Task<ReturnMessage> UndoSupplierInvoice([FromForm] long ID, [FromForm] string InvoiceNo )
        {
            var errors = await _Webcontext.ValidateAsync("spFMS_PaymentRequest", new { Option = "VAL_UNDO", ID });
            if (errors.Any())
                return DeleteError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spFMS_PaymentRequest", new { Option = "UNDO", ID, EntryBy = _appUser.CommonEmpNo });
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await new Document() { DocumentRoot = _commonHelper.GetDocumentRoot() }.DeleteByReference(DocumentReference.SupplierPayment, ID.ToString());

            await EventLog("SUPPLIER INVOICE", ActionType.Undo.ToString(), "PaymentRequestDetails", ID);
            return Message("Undo Successfully.");
        }

        [ValidateAction(Forms.Accounts.PaymentRequest, Rights.Approve)]
        public async Task<ReturnMessage> UndoTheCanceledSupplierInvoice([FromForm] long ID, [FromForm] string InvoiceNo)
        {
            var errors = await _Webcontext.ValidateAsync("spFMS_PaymentRequest", new { Option = "VAL_UNDO_THE_CANCELED_INVOICE", ID , InvoiceNo });
            if (errors.Any())
                return UndoError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spFMS_PaymentRequest", new { Option = "UNDO_THE_CANCELED_INVOICE", ID, EntryBy = _appUser.CommonEmpNo });
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await new Document() { DocumentRoot = _commonHelper.GetDocumentRoot() }.DeleteByReference(DocumentReference.SupplierPayment, ID.ToString());

            await EventLog("UNDO THE CANCELLED SUPPLIER INVOICE", ActionType.Undo.ToString(), "PaymentRequestDetails", ID);
            return Message("Undo the Cancelled Invoice Successfully.");
        }
        public async Task<List<string>> ReadPaymentCategory([FromForm] string PayeeType, [FromForm] long BusinessUnitId = 0)
        {
            if (BusinessUnitId == 0)
                return new List<string>(0);

            List<StringTypeModel> res = await _Webcontext.ExecuteSpAsync<StringTypeModel>("spFMS_PaymentRequest", new { Option = "GET_PAYMENT_CATEGORY", PayeeType, BusinessUnitId });
            return res.Select(x => x.Value).ToList();
        }
        public async Task<DataSourceResult> ReadAvailableService([DataSourceRequest] DataSourceRequest request, [FromForm] string ServiceCategory, [FromForm] long BusinessUnitId = 0)
        {
            var result = await _Webcontext.ExecuteSpAsync<BusinessService>("spFMS_PaymentRequest", new { Option = "GET_AVAIL_SERVICE", _appUser.CompanyId, ServiceCategory, BusinessUnitId });
            return await result.ToDataSourceResultAsync(request);
        }
    }
}
