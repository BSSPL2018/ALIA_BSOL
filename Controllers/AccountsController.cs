using BSOL.Core;
using BSOL.Core.Entities;
using BSOL.Core.Models.Accounts;
using BSOL.Core.Models.Common;
using BSOL.Helpers;
using BSOL.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using BSOL.Core.Models.Logitics;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Features;
using BSOL.Core.Helpers;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Vml.Office;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using ContractMaster = BSOL.Core.Models.Accounts.ContractMaster;

namespace BSOL.Controllers
{
    public class AccountsController : BaseController
    {
        private readonly ICommonHelper _commonHelper;
        public AccountsController(BSOLContext context, BSOLWebContext webContext, AppUser appUser, ICommonHelper commonHelper) : base(context, webContext, appUser)
        {
            _commonHelper = commonHelper;
        }

        #region Account Type
        [HttpPost]
        [ValidateAction(Forms.Accounts.ChartOfAccounts, Rights.View)]
        public async Task<DataSourceResult> ReadAccountTypes([DataSourceRequest] DataSourceRequest request)
        {
            return await _Webcontext.AccountTypes.Where(x => x.CompanyId == _appUser.CompanyId).ToDataSourceResultAsync(request);
        }

        [HttpPost]
        [ValidateAction(Forms.Accounts.ChartOfAccounts, Rights.Modify)]
        public async Task<DataSourceResult> UpdateAccountType([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "models")] IEnumerable<AccountType> datas)
        {
            foreach (var item in datas)
            {
                item.CompanyId = _appUser.CompanyId;
                if (!await item.SaveAsync())
                    ModelState.AddModelError(item.ErrorMessage);
            }

            return datas.ToDataSourceResult(request, ModelState);
        }

        [HttpPost]
        [ValidateAction(Forms.Accounts.ChartOfAccounts, Rights.Delete)]
        public async Task<ReturnMessage> DeleteAccountType([FromForm] AccountType data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Account Type deleted.");
        }
        #endregion

        #region Account Detail Type
        [HttpPost]
        [ValidateAction(Forms.Accounts.ChartOfAccounts, Rights.View)]
        public async Task<TreeDataSourceResult> ReadAccountDetailTypes([DataSourceRequest] DataSourceRequest request, [FromForm] long accountTypeId)
        {
            var result = await (from dt in _Webcontext.AccountDetailTypes
                                where dt.AccountTypeId == accountTypeId
                                select new AccountDetailType
                                {
                                    Id = dt.Id,
                                    ParentId = dt.ParentId,
                                    DetailType = dt.DetailType,
                                }).ToListAsync();
            return await result.ToTreeDataSourceResultAsync(request, e => e.Id, e => e.ParentId, e => e);
        }

        [HttpPost]
        [ValidateAction(Forms.Accounts.ChartOfAccounts, Rights.Modify)]
        public async Task<TreeDataSourceResult> UpdateAccountDetailType([DataSourceRequest] DataSourceRequest request, [FromForm] long accountTypeId, [FromForm] string accountType, [FromForm][Bind(Prefix = "models")] IEnumerable<AccountDetailType> datas)
        {
            foreach (var item in datas)
            {
                item.AccountTypeId = accountTypeId;
                item.Type = accountType;
                if (!await item.SaveAsync())
                    ModelState.AddModelError(item.ErrorMessage);
            }

            return datas.ToTreeDataSourceResult(request, ModelState);
        }

        [HttpPost]
        [ValidateAction(Forms.Accounts.ChartOfAccounts, Rights.Delete)]
        public async Task<TreeDataSourceResult> DeleteAccountDetailType([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "models")] IEnumerable<AccountDetailType> datas)
        {
            foreach (var item in datas)
            {
                if (!await item.RemoveAsync())
                    ModelState.AddModelError(item.ErrorMessage);
            }

            return datas.ToTreeDataSourceResult(request, ModelState);
        }
        #endregion

        #region Account Settings
        [HttpPost]
        [ValidateAction(Forms.Accounts.ChartOfAccounts, Rights.View)]
        public async Task<AccountSetting> GetAccountSetting()
        {
            return await _Webcontext.AccountSettings.FindAsync(_appUser.CompanyId);
        }

        [HttpPost]
        [ValidateAction(Forms.Accounts.ChartOfAccounts, Rights.Modify)]
        public async Task<ReturnMessage> UpdateAccountSetting([FromForm] AccountSetting data)
        {
            if (!await data.SaveAsync())
                return SaveError(data.ErrorMessage);

            return Message("Account Settings Saved");
        }
        #endregion

        #region Invoice Types/GST Settings

        #region Invoice Types

        [HttpPost]
        [ValidateAction(Forms.Accounts.ChartOfAccounts, Rights.View)]
        public async Task<DataSourceResult> ReadInvoiceTypes([DataSourceRequest] DataSourceRequest request)
        {
            var result = (from it in _Webcontext.InvoiceTypes
                          join ia in _Webcontext.Accounts on it.IncomeAccountId equals ia.Id
                          join ea in _Webcontext.Accounts on it.ExpenseAccountId equals ea.Id
                          join da in _Webcontext.Accounts on it.DiscountAccountId equals da.Id
                          join ra in _Webcontext.Accounts on it.ReturnAccountId equals ra.Id
                          select new
                          {
                              it.Id,
                              it.CompanyId,
                              it.Type,
                              it.IncomeAccountId,
                              it.ExpenseAccountId,
                              it.DiscountAccountId,
                              it.ReturnAccountId,
                              IncomeAccountName = ia.AccountName,
                              ExpenseAccountName = ea.AccountName,
                              DiscountAccountName = da.AccountName,
                              ReturnAccountName = ra.AccountName
                          }).ToList();
            return await result.ToDataSourceResultAsync(request);
        }

        [HttpPost]
        [ValidateAction(Forms.Accounts.ChartOfAccounts, Rights.Modify)]
        public async Task<DataSourceResult> UpdateInvoiceTypes([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "models")] IEnumerable<InvoiceType> datas)
        {
            foreach (var invoiceType in datas)
            {
                if (invoiceType.Id == 0)
                    invoiceType.CompanyId = _appUser.CompanyId;
                if (!await invoiceType.SaveAsync())
                    ModelState.AddModelError(invoiceType.ErrorMessage);
            }
            return datas.ToDataSourceResult(request, ModelState);
        }

        [HttpPost]
        [ValidateAction(Forms.Accounts.ChartOfAccounts, Rights.Delete)]
        public async Task<ReturnMessage> DeleteInvoiceType([FromForm] InvoiceType data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Invoice type deleted successfully.");
        }
        #endregion

        #region GST Settings

        [HttpPost]
        [ValidateAction(Forms.Accounts.ChartOfAccounts, Rights.View)]
        public async Task<DataSourceResult> ReadGSTSettings([DataSourceRequest] DataSourceRequest request)
        {
            var result = (from gst in _Webcontext.GSTSettings
                          join gin in _Webcontext.Accounts on gst.GSTInAccountId equals gin.Id
                          join gout in _Webcontext.Accounts on gst.GSTOutAccountId equals gout.Id
                          select new
                          {
                              gst.Id,
                              gst.Name,
                              gst.Percentage,
                              gst.GSTInAccountId,
                              gst.GSTOutAccountId,
                              GSTInAccount = gin.AccountName,
                              GSTOutAccount = gout.AccountName,
                              gst.IsDefault
                          }).ToList();
            return await result.ToDataSourceResultAsync(request);
        }

        [HttpPost]
        [ValidateAction(Forms.Accounts.ChartOfAccounts, Rights.Modify)]
        public async Task<DataSourceResult> UpdateGSTSettings([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "models")] IEnumerable<GSTSetting> datas)
        {
            foreach (var gstSetting in datas)
            {
                if (!await gstSetting.SaveAsync())
                    ModelState.AddModelError(gstSetting.ErrorMessage);
            }
            return datas.ToDataSourceResult(request, ModelState);
        }

        [HttpPost]
        [ValidateAction(Forms.Accounts.ChartOfAccounts, Rights.Delete)]
        public async Task<ReturnMessage> DeleteGSTSetting([FromForm] GSTSetting data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("GST setting deleted successfully.");
        }
        #endregion

        #endregion

        #region Accounts
        [HttpPost]
        [ValidateAction(Forms.Accounts.ChartOfAccounts, Rights.View)]
        public async Task<TreeDataSourceResult> ReadAccounts([DataSourceRequest] DataSourceRequest request)
        {
            var result = await _Webcontext.ExecuteSpAsync<ChartOfAccountModel>("spBSOL_ChartOfAccounts", new { _appUser.CompanyId });
            return await result.ToTreeDataSourceResultAsync(request, e => e.Id, e => e.ParentId, e => e);
        }

        [HttpPost]
        [ValidateAction(Forms.Accounts.ChartOfAccounts, Rights.View)]
        public async Task<Account> GetAccount([FromForm] long id)
        {
            return await (from a in _Webcontext.Accounts
                          join dt in _Webcontext.AccountDetailTypes on a.AccountDetailTypeId equals dt.Id
                          join t in _Webcontext.AccountTypes on dt.AccountTypeId equals t.Id
                          where a.Id == id
                          select new Account
                          {
                              Id = a.Id,
                              AccountDetailTypeId = a.AccountDetailTypeId,
                              AccountName = a.AccountName,
                              Code = a.Code,
                              Currency = a.Currency,
                              ActualCurrency = a.ActualCurrency,
                              OpeningBalance = a.OpeningBalance,
                              BalanceOn = a.BalanceOn,
                              AccountTypeId = dt.AccountTypeId,
                              DetailType = dt.DetailType
                          }).FirstOrDefaultAsync();
        }

        [HttpPost]
        [ValidateAction(Forms.Accounts.ChartOfAccounts, Rights.Modify)]
        public async Task<ReturnMessage> SaveAccount([FromForm] Account data)
        {
            if (!await data.SaveAsync())
                return SaveError(data.ErrorMessage);

            return Message("Account details saved.", data.Id);
        }

        [HttpPost]
        [ValidateAction(Forms.Accounts.ChartOfAccounts, Rights.Delete)]
        public async Task<ReturnMessage> DeleteAccount([FromForm] Account data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Account deleted.");
        }
        #endregion

        #region Payments
        [HttpPost]
        public async Task<SupplierPaymentTypeModel> GetSupplierBalance([FromForm] long Id)
        {
            return await _Webcontext.ExecuteSingleAsync<SupplierPaymentTypeModel>("spBSOL_SupplierPayment", new { Option = "GET_BALANCE", Id });
        }
        [HttpPost]
        [ValidateAction(Forms.Accounts.PaymentVoucher, Rights.View)]
        public async Task<DataSourceResult> ReadPaymentRequestList([DataSourceRequest] DataSourceRequest request, [FromForm] int statusFilter = 0)
        {
            var result = await _Webcontext.ExecuteSpAsync<PayoutListModel>("spBSOL_SupplierPayment", new { Option = "SELECT", statusFilter });
            return await result.ToDataSourceResultAsync(request);
        }
        [HttpPost]
        public async Task<DataSourceResult> ReadPaymentDetailsList([DataSourceRequest] DataSourceRequest request, [FromForm] int Id = 0)
        {
            var result = await _Webcontext.ExecuteSpAsync<PayoutListDetailsModel>("spBSOL_SupplierPayment", new { Option = "SELECT_INVOICE_DETAILS", Id });
            return await result.ToDataSourceResultAsync(request);
        }
        [HttpPost]
        [ValidateAction(Forms.Accounts.PaymentVoucher, Rights.Edit)]
        public async Task<ReturnMessage> UpdatePaymentReceived([FromForm] long Id, [FromForm] string RefNoFormatted, [FromForm] string ReceivedBy, [FromForm] DateTime ReceivedOn)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_SupplierPayment", new { Option = "VAL_RECEIVE", Id, ReceivedBy, ReceivedOn });
            if (errors.Any())
                return ApproveError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_SupplierPayment", new { Option = "RECEIVE", Id, ReceivedBy, ReceivedOn });
            if (retVal <= 0)
                return Error();

            await EventLog("Accounts - Payment Voucher", "UPDATE_RECEIVE", Id, RefNoFormatted);
            return Message("Payment request received.");
        }
        [HttpPost]
        [ValidateAction(Forms.Accounts.PaymentVoucher, Rights.Edit)]
        public async Task<ReturnMessage> RevertPaymentReceived([FromForm] long Id, [FromForm] string RefNoFormatted)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_SupplierPayment", new { Option = "VAL_UNDO_RECEIVE", Id });
            if (errors.Any())
                return ApproveError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_SupplierPayment", new { Option = "UNDO_RECEIVE", Id });
            if (retVal <= 0)
                return Error();

            await EventLog("Accounts - Payment Voucher", "UNDO_RECEIVE", Id, RefNoFormatted);
            return Message("Undo Successfully.");
        }

        [HttpPost]
        [ValidateAction(Forms.Accounts.PaymentVoucher, Rights.Edit)]
        public async Task<ReturnMessage> UpdatePaymentCreation([FromForm] long Id, [FromForm] string RefNoFormatted, [FromForm] string PaidBy, [FromForm] string HandOverTo, [FromForm] DateTime PaidOn, [FromForm] string ChequeNo, [FromForm] DateTime? ChequeDate, [FromForm] List<PaymentAccountDetail> accLst)
        {
            var duplicate = accLst.AsEnumerable().GroupBy(x => new { x.CreditTo, x.DebitTo }).Select(y => new
            {
                CreditTo = y.Key.CreditTo,
                DebitTo = y.Key.DebitTo,
                TotalCnt = y.ToList()
            });

            if (duplicate.Any(x => x.TotalCnt.Count > 1))
                return Error("Credit and debit account duplicate.");

            List<SqlParameter> sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@Option", "UPDATE_PAID"),
                new SqlParameter("@Id", Id),
                new SqlParameter("@PaidBy", PaidBy),
                new SqlParameter("@PaidOn", PaidOn),
                new SqlParameter("@ChequeNo", ChequeNo),
                new SqlParameter("@ChequeDate", ChequeDate),
                new SqlParameter("@HandOverTo", HandOverTo)
            };

            long retVal = await _Webcontext.ExecuteSqlNonQueryWithReturnAsync<long>("spBSOL_SupplierPayment", sqlParams);
            if (retVal <= 0)
                return Error();

            await EventLog("Accounts - Payment Voucher", "UPDATE_PAYMENT", Id, RefNoFormatted);

            foreach (var data in accLst)
            {
                long id = await _Webcontext.ExecuteNonQueryWithReturnAsync<long>("spBSOL_SupplierPayment", new
                {
                    Option = "UPDATE_ACCOUNT",
                    Id,
                    PaymentAccountId = data.ID,
                    data.CreditTo,
                    data.DebitTo,
                    SettledToAccount = data.Amount,
                    data.ShopGroupId
                });

                if (id <= 0)
                    return DBError();

                await EventLog("Accounts - Payment Voucher", "UPDATE_PAYMENT_To_Account", Id, RefNoFormatted, data.DebitToName, data.CreditToName, data.Amount);
            }


            return MessageWithId("Payment creation successfully.", Id);
        }
        [HttpPost]
        [ValidateAction(Forms.Accounts.PaymentVoucher, Rights.Edit)]
        public async Task<ReturnMessage> RevertPaymentCreation([FromForm] long Id, [FromForm] string RefNoFormatted)
        {
            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_SupplierPayment", new { Option = "UNDO_PAID", Id });
            if (retVal <= 0)
                return Error();

            await new Document() { DocumentRoot = _commonHelper.GetDocumentRoot() }.DeleteByReference(DocumentReference.SupplierPayment, Id.ToString());

            await EventLog("Accounts - Payment Voucher", "UNDO_PAYMENT", Id, RefNoFormatted);
            return Message("Undo successfully.");
        }
        [HttpPost]
        [ValidateAction(Forms.Accounts.PaymentVoucher, Rights.Reject)]
        public async Task<ReturnMessage> UpdatePaymentRejection([FromForm] long Id, [FromForm] string RefNoFormatted, [FromForm] string RemarksForReject)
        {
            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_SupplierPayment", new { Option = "UPDATE_REJECT", Id, EntryBy = _appUser.CommonEmpNo, RemarksForReject });
            if (retVal <= 0)
                return Error();

            await EventLog("Accounts - Payment Voucher", "PAYMENT_REJECT", Id, RefNoFormatted);
            return Message("Payment Rejected.");
        }
        #endregion
        #region Expenses
        [HttpPost]
        public async Task<PaymentRequestValue> GetRequestedPaymentDetails([FromForm] long ID)
        {
            return await _Webcontext.ExecuteSingleAsync<PaymentRequestValue>("spBSOL_SupplierPayment", new { Option = "GET_PAYMENT_DETAILS", PaymentRequestID = ID });
        }

        #endregion

        #region Remittance
        [HttpPost]
        [ValidateAction(Forms.Accounts.Remittance, Rights.View)]
        public async Task<DataSourceResult> ReadRemittance([DataSourceRequest] DataSourceRequest request, [FromForm] int StatusFilter, [FromForm] DateTime FromDate, [FromForm] DateTime ToDate)
        {
            var result = await _Webcontext.ExecuteSpAsync<RemittanceModel>("spFMS_Remittance", new { Option = "SELECT", _appUser.CompanyId, StatusFilter, FromDate, ToDate });
            return await result.ToDataSourceResultAsync(request);
        }
        [HttpPost]
        public async Task<DataSourceResult> ReadRemittanceDetailsList([DataSourceRequest] DataSourceRequest request, [FromForm] DateTime FromDate, [FromForm] DateTime ToDate, [FromForm] long ShopId)
        {
            var result = await _Webcontext.ExecuteSpAsync<RemittanceDetailsModel>("spFMS_Remittance", new { Option = "TRANSACTION_DETAILS", _appUser.CompanyId, FromDate, ToDate, ShopId });
            return await result.ToDataSourceResultAsync(request);
        }
        [HttpPost]
        [ValidateAction(Forms.Accounts.SettlementDetails, Rights.View)]
        public async Task<DataSourceResult> ReadSettlementDetailsList([DataSourceRequest] DataSourceRequest request, [FromForm] DateTime FromDate, [FromForm] DateTime ToDate, [FromForm] long ShopId)
        {
            var result = await _Webcontext.ExecuteSpAsync<SettlementDetailsModel>("spHP_SelectSettementDetails", new { FromDate, ToDate, ShopID = ShopId });
            return await result.ToDataSourceResultAsync(request);
        }
        [HttpPost]
        [ValidateAction(Forms.Accounts.Remittance, Rights.Modify)]
        public async Task<ReturnMessage> SaveRemittance([FromForm] Remittance data, [FromForm] int StatusFilter)
        {
            List<SqlParameter> sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@Option", "VAL_SAVE"),
                new SqlParameter("@ShopGroupID", data.ShopGroupID),
                new SqlParameter("@CompanyId", _appUser.CompanyId),
                new SqlParameter("@TransactionID", data.TransactionID),
                new SqlParameter("@TransactionOn", data.TransactionOn),
                new SqlParameter("@CustomerName", data.CustomerName),
                new SqlParameter("@NicNo", data.NicNo),
                new SqlParameter("@ContactNo", data.ContactNo),
                new SqlParameter("@FromBankID", data.FromBankID),
                new SqlParameter("@FromBank", data.FromBank),
                new SqlParameter("@FromAccountName", data.FromAccountName),
                new SqlParameter("@FromAccountNo", data.FromAccountNo),
                new SqlParameter("@ToAccountID", data.ToAccountId),
                new SqlParameter("@ToBankName", data.ToBankName),
                new SqlParameter("@ToAccountNo", data.ToAccountNo),
                new SqlParameter("@Currency", data.Currency),
                new SqlParameter("@Amount", data.Amount),
                new SqlParameter("@ShopGroupViberNo", data.ShopGroupViberNo),
                new SqlParameter("@Remarks", data.Remarks),
                new SqlParameter("@AccountsRemarks", data.AccountsRemarks),
                new SqlParameter("@EntryBy", _appUser.CommonEmpNo),
                new SqlParameter("@ID", data.ID),
                new SqlParameter("@CustomerID", data.CustomerID),
                new SqlParameter("@StatusFilter", StatusFilter)
            };

            var errors = await _Webcontext.ValidateSqlAsync("spFMS_Remittance", sqlParams);
            if (errors.Any())
                return SaveError(errors);

            sqlParams[0].Value = data.ID > 0 ? "UPDATE" : "INSERT";

            long id = await _Webcontext.ExecuteSqlNonQueryWithReturnAsync<long>("spFMS_Remittance", sqlParams);
            if (id <= 0)
                return Error();

            await EventLog("Accounts - Remittance", "SAVED", data.TransactionID, data.Amount);
            return Message("Saved successfully.", id);
        }
        [HttpPost]
        [ValidateAction(Forms.Accounts.Remittance, Rights.Delete)]
        public async Task<ReturnMessage> DeleteRemittance([FromForm] long ID, [FromForm] string RefNo)
        {
            var errors = await _Webcontext.ValidateAsync("spFMS_Remittance", new { Option = "VAL_DELETE", ID });
            if (errors.Any())
                return DeleteError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spFMS_Remittance", new { Option = "DELETE", ID });
            if (retVal <= 0)
                return Error();

            await new Document() { DocumentRoot = _commonHelper.GetDocumentRoot() }.DeleteByReference(DocumentReference.Remittance, ID.ToString());

            await EventLog("Accounts - Remittance", "DELETE", ID, RefNo);
            return Message("Remittance deleted.");
        }

        [HttpPost]
        [ValidateAction(Forms.Accounts.Remittance, Rights.Approve)]
        public async Task<ReturnMessage> ApproveRemittance([FromForm] long ID, [FromForm] string RefNo, [FromForm] int StatusFilter)
        {
            var errors = await _Webcontext.ValidateAsync("spFMS_Remittance", new { Option = "VAL_VERIFY", ID, StatusFilter });
            if (errors.Any())
                return ApproveError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spFMS_Remittance", new { Option = "APPROVE", ID, ApprovedBy = _appUser.CommonEmpNo, StatusFilter });
            if (retVal <= 0)
                return Error();

            await EventLog("Accounts - Remittance", StatusFilter == 0 ? "OUTLET - APPROVE" : StatusFilter == 1 ? "ACCOUNTS - APPROVE" : "ACCOUNTS - RECONCILIATION", ID, RefNo);
            return Message("Remittance verified.");
        }

        [HttpPost]
        [ValidateAction(Forms.Accounts.Remittance, Rights.Reject)]
        public async Task<ReturnMessage> RevertRemittance([FromForm] long ID, [FromForm] string RefNo, [FromForm] int StatusFilter)
        {
            var errors = await _Webcontext.ValidateAsync("spFMS_Remittance", new { Option = "VAL_REVERT", ID, StatusFilter });
            if (errors.Any())
                return ApproveError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spFMS_Remittance", new { Option = "UNDO", ID, StatusFilter });
            if (retVal <= 0)
                return Error();

            await EventLog("Accounts - Remittance", StatusFilter == 3 ? "REVERT - RECONCILIATION" : StatusFilter == 2 ? "ACCOUNTS - REVERT" : "OUTLET - REVERT", ID, RefNo);
            return Message("Remittance reverted.");
        }

        [HttpPost]
        public async Task<List<DropDownModel>> ReadShopGroup()
        {
            return await _Webcontext.ShopGroups.Select(x => new DropDownModel { Id = x.ID, Text = x.Name }).ToListAsync();
        }

        [HttpPost]
        public async Task<List<DropDownModel>> ReadCustomer()
        {
            List<DropDownModel> result = await _Webcontext.ExecuteSpAsync<DropDownModel>("spFMS_Remittance", new { Option = "GET_CUSTOMER" });
            return result;
        }

        [HttpPost]
        public async Task<List<DropDownModel>> ReadBank()
        {
            return await _Webcontext.Banks.Select(x => new DropDownModel { Id = x.ID, Text = x.BankName }).ToListAsync();
        }

        [HttpPost]
        public async Task<List<DropDownModel>> ReadToBank()
        {
            return await _Webcontext.Banks.Select(x => new DropDownModel { Id = x.ID, Text = x.BankName }).ToListAsync();
        }
        [HttpPost]
        public async Task<List<DropDownModel>> ReadToAccountNo([FromForm] long ToBankId)
        {
            return await _Webcontext.CompanyAccounts.Where(x => x.BankID == ToBankId).Select(x => new DropDownModel { Id = x.ID, Text = x.CompanyAccountNo }).ToListAsync();
        }
        [HttpPost]
        public async Task<List<Curency>> ReadCurrency()
        {
            return await _Webcontext.Currencies.Select(x => new Curency { Currency = x.Currency }).ToListAsync();
        }
        [HttpPost]
        public async Task<CustomDetailModel> ReadCustomerDetails([FromForm] long CustomerId)
        {
            return await _Webcontext.ExecuteSingleAsync<CustomDetailModel>("spFMS_Remittance", new { Option = "GET_CUSTOMER_DETAILS", CustomerID = CustomerId });
        }
        #endregion

        #region Bank

        [ValidateAction(Forms.Accounts.BankDetails, Rights.View)]
        public async Task<DataSourceResult> ReadBankList([DataSourceRequest] DataSourceRequest request)
        {
            var result = await (from b in _Webcontext.Banks
                                select new Bank
                                {
                                    ID = b.ID,
                                    BankName = b.BankName,
                                    BankCode = b.BankCode,
                                    EntryBy = b.EntryBy,
                                    EntryDate = b.EntryDate,
                                    CompanyID = b.CompanyID
                                }).ToListAsync();

            return await result.ToDataSourceResultAsync(request);
        }

        [ValidateAction(Forms.Accounts.BankDetails, Rights.Modify)]
        public async Task<DataSourceResult> UpdateBank([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "models")] IEnumerable<Bank> datas)
        {
            foreach (var item in datas)
            {
                item.CompanyID = _appUser.CompanyId;
                if (!await item.SaveAsync())
                    ModelState.AddModelError(item.ErrorMessage);
            }
            return datas.ToDataSourceResult(request, ModelState);
        }

        [ValidateAction(Forms.Accounts.BankDetails, Rights.Delete)]
        public async Task<ReturnMessage> DeleteBank([FromForm] Bank data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Bank details deleted.");
        }

        #endregion

        #region Company Account

        [ValidateAction(Forms.Accounts.BankDetails, Rights.View)]
        [HttpPost]
        public async Task<DataSourceResult> ReadCompanyAccount([DataSourceRequest] DataSourceRequest request)
        {
            var result = await (from b in _Webcontext.Banks
                                join c in _Webcontext.CompanyAccounts on b.ID equals c.BankID
                                select new CompanyAccount
                                {
                                    ID = c.ID,
                                    BankName = b.BankName,
                                    BranchName = c.BranchName,
                                    BankID = c.BankID,
                                    Currency = c.Currency,
                                    CompanyAccountName = c.CompanyAccountName,
                                    CompanyID = c.CompanyID,
                                    CompanyAccountNo = c.CompanyAccountNo,
                                    BankCharge = c.BankCharge,
                                    ChargeBasedOn = c.ChargeBasedOn,
                                    EntryBy = c.EntryBy,
                                    EntryDate = c.EntryDate
                                }).ToListAsync();

            return await result.ToDataSourceResultAsync(request);
        }

        [ValidateAction(Forms.Accounts.BankDetails, Rights.Modify)]
        [HttpPost]
        public async Task<DataSourceResult> UpdateCompanyAccount([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "models")] IEnumerable<CompanyAccount> datas)
        {
            foreach (var item in datas)
            {
                item.CompanyID = _appUser.CompanyId;
                if (!await item.SaveAsync())
                    ModelState.AddModelError(item.ErrorMessage);
            }
            return datas.ToDataSourceResult(request, ModelState);
        }

        [ValidateAction(Forms.Accounts.BankDetails, Rights.Delete)]
        public async Task<ReturnMessage> DeleteCompanyAccount([FromForm] Bank data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Company Account details deleted.");
        }
        [HttpPost]
        public async Task<List<string>> GetCurrency()
        {
            return await _Webcontext.Currencies.Select(x => x.Currency).ToListAsync();
        }
        #endregion
        #region BusinessService
        [ValidateAction(Forms.Accounts.BusinessServices, Rights.View)]
        public async Task<TreeDataSourceResult> ReadBusiness([DataSourceRequest] DataSourceRequest request)
        {

            var result = await (from b in _Webcontext.BusinessServices
                                join s in _Webcontext.ShopGroups on b.BusinessUnitId equals s.ID
                                join lacd in _Webcontext.Accounts on b.DebitTo equals lacd.Id into grpAC
                                from acd in grpAC.DefaultIfEmpty()
                                join lac in _Webcontext.Accounts on b.CreditTo equals lac.Id into grpCAC
                                from acc in grpCAC.DefaultIfEmpty()
                                select new
                                {
                                    ID = b.ID,
                                    BusinessUnitId = b.BusinessUnitId,
                                    ParentId = b.ParentId,
                                    BusinessName = s.Name,
                                    ServiceCategory = b.ServiceCategory,
                                    ServiceNo = b.ServiceNo,
                                    ServiceName = b.ServiceName,
                                    CreatedBy = b.CreatedBy,
                                    CreatedOn = b.CreatedOn,
                                    DebitTo = b.DebitTo,
                                    CreditTo = b.CreditTo,
                                    DebitAccountName = string.Concat(acd.AccountName, " - ", acd.Code),
                                    CreditAccountName = string.Concat(acc.AccountName, " - ", acc.Code)
                                }).OrderByDescending(x => x.ID).ToListAsync();
            //return result.ToDataSourceResult(request);
            return result.ToTreeDataSourceResult(request, e => e.ID, e => e.ParentId, e => e);
        }
        [ValidateAction(Forms.Accounts.BusinessServices, Rights.Modify)]
        public async Task<ReturnMessage> SaveBusiness([FromForm] BusinessService data)
        {

            if (!await data.SaveAsync())
                return SaveError(data.ErrorMessage);

            return Message("BusinessServices details saved.", data.ID);
        }
        [ValidateAction(Forms.Accounts.BusinessServices, Rights.Delete)]
        public async Task<ReturnMessage> DeleteBusiness([FromForm] BusinessService data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("BusinessService details deleted.");
        }
        #endregion

        #region Contract Policy
        public async Task<List<string>> ReadContractType()
        {
            return await _Webcontext.ContractMasters.Where(x => x.ContractType != null).Select(x => x.ContractType).Distinct().ToListAsync();

        }
        public async Task<object> ReadVenorList()
        {
            return await (from s in _Webcontext.Suppliers
                          select new
                          {
                              Id = s.Id,
                              Text = s.SupplierName + "-" + s.SupplierCode
                          }).Distinct().ToListAsync();
        }
        public async Task<List<string>> ReadCurrencies()
        {
            return await _Webcontext.Currencies.Select(x => x.Currency).ToListAsync();
        }
        public async Task<List<string>> ReadExpireNotifyTo()
        {
            return await _Webcontext.Employees.Where(x => !string.IsNullOrEmpty(x.EmailID) && x.Active).Select(x => x.EmailID).Distinct().ToListAsync();
        }
        [HttpPost]
        [ValidateAction(Forms.Accounts.ContractList, Rights.Modify)]
        public async Task<ReturnMessage> SaveContractPolicy([FromForm] ContractMaster data)
        {
            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "VAL_SAVE"),
                        new SqlParameter("@ID",data.ID),
                        new SqlParameter("@ContractRefNo", data.ContractRefNo),
                        new SqlParameter("@ContractType", data.ContractType),
                        new SqlParameter("@ContractName", data.ContractName),
                        new SqlParameter("@VendorID", data.VendorID),
                        new SqlParameter("@Details", data.Details),
                        new SqlParameter("@Amount", data.Amount),
                        new SqlParameter("@Currency", data.Currency),
                        new SqlParameter("@ContractFrom", data.ContractFrom),
                        new SqlParameter("@ContractTo", data.ContractTo),
                        new SqlParameter("@ExpireNotifyDays", data.ExpireNotifyDays),
                        new SqlParameter("@ExpireNotifyTo", data.ExpireNotifyTo),
                        new SqlParameter("@CreatedBy",  _appUser.CommonEmpNo),
                        new SqlParameter("@ContractAmount", data.ContractAmount),
                    };
            var lstError = await _Webcontext.ValidateSqlAsync("spFMS_ContractPolicy", sqlParams);
            if (lstError.Any())
                return SaveError(lstError);

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.AddRange(sqlParams.Select(x => ((ICloneable)x).Clone() as SqlParameter));
            sqlParameters[0].Value = data.ID > 0 ? "UPDATE" : "INSERT";

            var id = await _Webcontext.ExecuteSqlNonQueryWithReturnAsync<long>("spFMS_ContractPolicy", sqlParameters);

            if (id <= 0)
                return Error();

            await EventLog("Accounts - Contract Policy", "SAVED", data.ContractRefNo, data.ContractType.ToString(), data.ContractName, data.ContractFrom, data.ContractTo, data.Amount);
            return Message("Saved successfully.", id);
        }
        [HttpPost]
        [ValidateAction(Forms.Accounts.ContractList, Rights.Delete)]
        public async Task<ReturnMessage> DeleteContractPolicy([FromForm] ContractMaster data)
        {
            var errors = await _Webcontext.ValidateAsync("spFMS_ContractPolicy", new { Option = "VAL_DELETE", data.ID });
            if (errors.Any())
                return DeleteError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spFMS_ContractPolicy", new { Option = "DELETE", data.ID });
            if (retVal <= 0)
                return Error();

            await EventLog("Accounts - Contract Policy", "DELETED", data.ContractRefNo, data.ContractFrom, data.ContractTo, data.Amount);
            return Message("Deleted Successfully.");
        }
        [ValidateAction(Forms.Accounts.ContractList, Rights.Approve)]
        public async Task<ReturnMessage> ApproveContractPolicy([FromForm] ContractMaster data)
        {
            int retVal = await _Webcontext.ExecuteNonQueryAsync("spFMS_ContractPolicy", new { Option = "APPROVE", data.ID, CreatedBy = _appUser.CommonEmpNo });
            if (retVal <= 0)
                return Error();

            await EventLog("Accounts - Contract Policy", "APPROVE", data.ContractRefNo, data.ContractFrom, data.ContractTo, data.Amount);
            return Message("Approved Successfully.");
        }
        [ValidateAction(Forms.Accounts.ContractList, Rights.Approve)]
        public async Task<ReturnMessage> UndoContractPolicy([FromForm] ContractMaster data)
        {
            var errors = await _Webcontext.ValidateAsync("spFMS_ContractPolicy", new { Option = "VAL_UNDO", data.ID });
            if (errors.Any())
                return UndoError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spFMS_ContractPolicy", new { Option = "UNDO", data.ID });
            if (retVal <= 0)
                return Error();

            await EventLog("Accounts - Contract Policy", "UNDO", data.ContractRefNo, data.ContractFrom, data.ContractTo, data.Amount);
            return Message("Undo Successfully.");
        }
        [HttpPost]
        public async Task<DataSourceResult> ReadContractList([DataSourceRequest] DataSourceRequest Request, [FromForm] int StatusFilter)
        {
            List<ContractMasterModel> contractPolicy = await _Webcontext.ExecuteSpAsync<ContractMasterModel>("spFMS_ContractPolicy", new { Option = "SELECT", StatusFilter });
            return contractPolicy.ToDataSourceResult(Request);
        }

        [HttpPost]
        [ValidateAction(Forms.Accounts.ContractList, Rights.Modify)]
        public async Task<ReturnMessage> UpdateContractPolicy([FromForm] long ID, [FromForm] string RContractRefNo, [FromForm] DateTime RContractFrom, [FromForm] DateTime RContractTo, [FromForm] string RCurrency, [FromForm] decimal RAmount, [FromForm] string RExpireNotifyTo, [FromForm] int RExpireNotifyDays)
        {

            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "VAL_SAVE_RENEWAL"),
                        new SqlParameter("@ID",ID),
                        new SqlParameter("@Amount", RAmount),
                        new SqlParameter("@Currency", RCurrency),
                        new SqlParameter("@ContractFrom", RContractFrom),
                        new SqlParameter("@ContractTo", RContractTo),
                        new SqlParameter("@ExpireNotifyDays", RExpireNotifyDays),
                        new SqlParameter("@ExpireNotifyTo", RExpireNotifyTo),
                         new SqlParameter("@ContractRefNo", RContractRefNo),
                        new SqlParameter("@CreatedBy",  _appUser.CommonEmpNo),
                    };
            var lstError = await _Webcontext.ValidateSqlAsync("spFMS_ContractPolicy", sqlParams);
            if (lstError.Any())
                return SaveError(lstError);

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.AddRange(sqlParams.Select(x => ((ICloneable)x).Clone() as SqlParameter));
            sqlParameters[0].Value = "UPDATE_RENEWAL";

            var id = await _Webcontext.ExecuteSqlNonQueryAsync("spFMS_ContractPolicy", sqlParameters);

            if (id <= 0)
                return Error();

            await EventLog("Accounts - Contract Policy", "CONTRACT-RENEWAL", RContractRefNo, RContractFrom, RContractTo, RCurrency, RAmount, RExpireNotifyDays, RExpireNotifyTo);
            return Message("Saved successfully.", id);
        }
        [ValidateAction(Forms.Accounts.ContractList, Rights.Modify)]
        public async Task<ReturnMessage> RevertContractPolicy([FromForm] ContractMaster data)
        {
            int retVal = await _Webcontext.ExecuteNonQueryAsync("spFMS_ContractPolicy", new { Option = "UNDO_RENEWAL", data.ID });
            if (retVal <= 0)
                return Error();

            await EventLog("Accounts - Contract Policy", "CONTRACT-RENEWAL-UNDO", data.ContractRefNo, data.ContractFrom, data.ContractTo, data.Currency, data.Amount, data.ExpireNotifyDays, data.ExpireNotifyTo);
            return Message("Revert Successfully.");
        }
        [HttpPost]
        public async Task<DataSourceResult> ReadContractExpiredList([DataSourceRequest] DataSourceRequest Request, [FromForm] long ID)
        {
            List<ContractExpiredList> contractExpiredPolicy = await _Webcontext.ExecuteSpAsync<ContractExpiredList>("spFMS_ContractPolicy", new { Option = "SELECT_CONTRACT_HISTORY", ID });
            return contractExpiredPolicy.ToDataSourceResult(Request);
        }
        #endregion 
    }
}
