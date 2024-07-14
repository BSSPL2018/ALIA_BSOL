using BSOL.Core;
using BSOL.Core.Models.Accounts;
using BSOL.Core.Models.Common;
using BSOL.Core.Models.HirePurchase;
using BSOL.Helpers;
using DocumentFormat.OpenXml.Office2010.Excel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Server;
using SqlMetaData = Microsoft.Data.SqlClient.Server.SqlMetaData;
using SqlDataRecord = Microsoft.Data.SqlClient.Server.SqlDataRecord;
using BSOL.Web.Helpers;
using DocumentFormat.OpenXml.Spreadsheet;
using BSOL.Core.Entities;

namespace BSOL.Controllers
{
    public class HirePurchaseController : BaseController
    {
        private readonly ICommonHelper _commonHelper;
        public HirePurchaseController(BSOLContext context, BSOLWebContext webContext, AppUser appUser, ICommonHelper commonHelper) : base(context, webContext, appUser)
        {
            _commonHelper = commonHelper;
        }
        [HttpPost]
        [ValidateAction(Forms.HirePurchases.ChequeRegistry, Rights.View)]
        //public async Task<DataSourceResult> ReadChequeList([DataSourceRequest] DataSourceRequest request, [FromForm] int StatusFilter, [FromForm] bool IsExpiredCheque)
        public async Task<DataSourceResult> ReadChequeList([DataSourceRequest] DataSourceRequest request, [FromForm] string ChequeStatus, [FromForm] bool IsExpiredCheque)
        {
            var result = await _Webcontext.ExecuteSpAsync<ChequeListModel>("spHP_SelectChequeMaster", new { ChequeStatus, IsExpiredCheque });
            return await result.ToDataSourceResultAsync(request);
        }
        [HttpPost]
        [ValidateAction(Forms.HirePurchases.ChequeRegistry, Rights.View)]
        public async Task<DataSourceResult> ReadChequeDetailsList([DataSourceRequest] DataSourceRequest request, [FromForm] DateTime FromDate, [FromForm] DateTime ToDate)
        {
            var result = await _Webcontext.ExecuteSpAsync<ChequeDetailsModel>("spHP_ChequeList", new { Option = "SELECT_DETAILS", _appUser.CompanyId, FromDate, ToDate });
            return await result.ToDataSourceResultAsync(request);
        }
        [HttpPost]
        public async Task<List<DropDownModel>> ReadCustomer()
        {
            //List<DropDownModel> result = await _Webcontext.ExecuteSpAsync<DropDownModel>("spHP_ChequeList", new { Option = "GET_CUSTOMER" });
            //return result;

            return await (from c in _context.CustomerDetails
                          where c.Cancel_Flag == false
                          select new DropDownModel
                          {
                              Id = c.tbl_ID,
                              Text = c.Cust_Name_EN + "-" + c.Cust_Ref
                          }).ToListAsync();
        }

        [HttpPost]
        public async Task<List<DropDownModel>> ReadAgreementNo([FromForm] long CustomerID)
        {
            List<DropDownModel> result = await _Webcontext.ExecuteSpAsync<DropDownModel>("spHP_ChequeList", new { Option = "GET_AGREEMENT_NO", CustomerID });
            return result;
        }
        [HttpPost]
        [ValidateAction(Forms.HirePurchases.ChequeRegistry, Rights.Modify)]
        public async Task<ReturnMessage> AddCheques([FromForm] List<ChequeList> datas, [FromForm] long? ID, [FromForm] string CustomerName = "", [FromForm] string AgreementNo = "")
        {
            var tableSchemas = new List<SqlMetaData>() {
                  new SqlMetaData("BankID", SqlDbType.BigInt),
                  new SqlMetaData("BankName", SqlDbType.VarChar,100),
                  new SqlMetaData("AccountName", SqlDbType.VarChar,100),
                  new SqlMetaData("AccountNo", SqlDbType.VarChar,50),
                  new SqlMetaData("ChequeNo", SqlDbType.VarChar,50),
                  new SqlMetaData("Amount", SqlDbType.Money),
                  new SqlMetaData("ID", SqlDbType.BigInt),
                  new SqlMetaData("Currency", SqlDbType.VarChar,50),
                  new SqlMetaData("SentToBank", SqlDbType.Date),
                  new SqlMetaData("ChequeDate", SqlDbType.Date),
                  new SqlMetaData("Remarks", SqlDbType.VarChar,500),
                  new SqlMetaData("CustomerID", SqlDbType.BigInt),
                  new SqlMetaData("AgreementID", SqlDbType.BigInt),
                  new SqlMetaData("ReferenceNo", SqlDbType.VarChar,100),
                  new SqlMetaData("ReferenceDate", SqlDbType.Date)
            }.ToArray();

            var sqlDataRecords = new List<SqlDataRecord>();
            datas.ForEach(x =>
            {
                SqlDataRecord sqlDataRecord = new SqlDataRecord(tableSchemas);
                sqlDataRecord.SetInt64(0, (long)x.BankID);
                sqlDataRecord.SetString(1, x.BankName);
                sqlDataRecord.SetString(2, x.AccountName);
                sqlDataRecord.SetString(3, x.AccountNo);
                sqlDataRecord.SetString(4, x.ChequeNo);
                sqlDataRecord.SetDecimal(5, x.Amount);
                sqlDataRecord.SetInt64(6, (long)x.ID);
                sqlDataRecord.SetString(7, x.Currency);
                sqlDataRecord.SetDateTime(8, (DateTime)x.SentToBank);
                sqlDataRecord.SetDateTime(9, (DateTime)x.ChequeDate);
                sqlDataRecord.SetString(10, x.Remarks ?? "");
                sqlDataRecord.SetInt64(11, (long)x.CustomerID);
                sqlDataRecord.SetInt64(12, (long)x.AgreementID);
                sqlDataRecord.SetString(13, x.ReferenceNo ?? "");
                if (x.ReferenceDate.HasValue)
                    sqlDataRecord.SetDateTime(14, (DateTime)x.ReferenceDate);

                sqlDataRecords.Add(sqlDataRecord);
            });

            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "VAL_SAVE"),
                        new SqlParameter("@EntryBy", _appUser.CommonEmpNo),
                        new SqlParameter("@ID", ID),
                        new SqlParameter{
                                        ParameterName = "@ChequeList",
                                        Value = sqlDataRecords,
                                        SqlDbType = SqlDbType.Structured,
                                        TypeName = "dbo.ChequeList"
                        }
                    };

            var errors = await _Webcontext.ValidateSqlAsync("spHP_ChequeList", sqlParams);
            if (errors.Any())
                return SaveError(errors);

            sqlParams[0].Value = ID > 0 ? "UPDATE" : "INSERT";

            long id = await _Webcontext.ExecuteSqlNonQueryAsync("spHP_ChequeList", sqlParams);
            if (id <= 0)
                return Error();

            string chequelst = string.Join("/n", datas.AsEnumerable().Select(x => x.ChequeNo).ToList());
            await EventLog("Hire Purchase - Cheque Registry", ID > 0 ? "EDIT" : "INSERT", ID, CustomerName, chequelst);
            return Message("Cheque saved.");
        }

        [HttpPost]
        [ValidateAction(Forms.HirePurchases.ChequeRegistry, Rights.Approve)]
        public async Task<ReturnMessage> ProcessingCheque([FromForm] List<ChequeList> datas, [FromForm] string Option, [FromForm] DateTime? ProcessedOn, [FromForm] string BankName = "", [FromForm] string AccountNo = "", [FromForm] string DepositRemarks = "", [FromForm] string DepositedBy = "")
        {
            var tableSchemas = new List<SqlMetaData>() {
                  new SqlMetaData("BankID", SqlDbType.BigInt),
                  new SqlMetaData("BankName", SqlDbType.VarChar,100),
                  new SqlMetaData("AccountName", SqlDbType.VarChar,100),
                  new SqlMetaData("AccountNo", SqlDbType.VarChar,50),
                  new SqlMetaData("ChequeNo", SqlDbType.VarChar,50),
                  new SqlMetaData("Amount", SqlDbType.Money),
                  new SqlMetaData("ID", SqlDbType.BigInt),
                  new SqlMetaData("Currency", SqlDbType.VarChar,50),
                  new SqlMetaData("SentToBank", SqlDbType.Date),
                  new SqlMetaData("ChequeDate", SqlDbType.Date),
                   new SqlMetaData("Remarks", SqlDbType.VarChar,500),
                   new SqlMetaData("CustomerID", SqlDbType.BigInt),
                  new SqlMetaData("AgreementID", SqlDbType.BigInt),
                  new SqlMetaData("ReferenceNo", SqlDbType.VarChar,100),
                  new SqlMetaData("ReferenceDate", SqlDbType.Date)
        }.ToArray();

            var sqlDataRecords = new List<SqlDataRecord>();
            datas.ForEach(x =>
            {
                SqlDataRecord sqlDataRecord = new SqlDataRecord(tableSchemas);
                sqlDataRecord.SetInt64(0, (long)0);
                sqlDataRecord.SetString(1, (string)"");
                sqlDataRecord.SetString(2, (string)"");
                sqlDataRecord.SetString(3, (string)"");
                sqlDataRecord.SetString(4, x.ChequeNo);
                sqlDataRecord.SetDecimal(5, (decimal)0.0);
                sqlDataRecord.SetInt64(6, (long)x.ID);
                sqlDataRecord.SetString(7, (string)"");
                sqlDataRecord.SetDateTime(8, DateTime.Now);
                sqlDataRecord.SetDateTime(9, DateTime.Now);
                sqlDataRecord.SetString(10, (string)"");
                sqlDataRecord.SetInt64(11, (long)0);
                sqlDataRecord.SetInt64(12, (long)0);
                sqlDataRecord.SetString(13, (string)"");
                sqlDataRecord.SetDateTime(14, DateTime.Now);
                sqlDataRecords.Add(sqlDataRecord);
            });


            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                new SqlParameter("@Option", Option),
                new SqlParameter("@EntryBy", _appUser.CommonEmpNo),
                new SqlParameter("@DepositedBankName", BankName),
                new SqlParameter("@DepositedAccountNo", AccountNo),
                new SqlParameter("@DepositRemarks", DepositRemarks),
                new SqlParameter("@ProcessedOn", ProcessedOn),
                new SqlParameter("@ProcessedBy", DepositedBy),
                        new SqlParameter
                        {
                        ParameterName = "@ChequeList",
                        Value = sqlDataRecords,
                        SqlDbType = SqlDbType.Structured,
                        TypeName = "dbo.ChequeList"
                        }
            };

            string[] optionlst = { "UNDO" };
            if (optionlst.Any(x => x == Option))
            {
                sqlParams[0].Value = "VAL_UNDO";
                var errors = await _Webcontext.ValidateSqlAsync("spHP_ChequeList", sqlParams);
                if (errors.Any())
                    return Error(errors);
            }

            string[] optionUnDepositlst = { "UNDO_DEPOSIT" };
            if (optionUnDepositlst.Any(x => x == Option))
            {
                sqlParams[0].Value = "VAL_UNDO_DEPOSIT";
                var errors = await _Webcontext.ValidateSqlAsync("spHP_ChequeList", sqlParams);
                if (errors.Any())
                    return Error(errors);
            }

            string[] optiondepositlst = { "DEPOSIT" };
            if (optiondepositlst.Any(x => x == Option))
            {
                sqlParams[0].Value = "VAL_DEPOSIT";
                var errors = await _Webcontext.ValidateSqlAsync("spHP_ChequeList", sqlParams);
                if (errors.Any())
                    return Error(errors);
            }

            string[] optionclearlst = { "CLEAR" };
            if (optionclearlst.Any(x => x == Option))
            {
                sqlParams[0].Value = "VAL_CLEAR";
                var errors = await _Webcontext.ValidateSqlAsync("spHP_ChequeList", sqlParams);
                if (errors.Any())
                    return Error(errors);
            }

            string[] optionbouncelst = { "BOUNCE" };
            if (optionbouncelst.Any(x => x == Option))
            {
                sqlParams[0].Value = "VAL_BOUNCE";
                var errors = await _Webcontext.ValidateSqlAsync("spHP_ChequeList", sqlParams);
                if (errors.Any())
                    return Error(errors);
            }

            sqlParams[0].Value = Option;

            long id = await _Webcontext.ExecuteSqlNonQueryAsync("spHP_ChequeList", sqlParams);
            if (id <= 0)
                return Error();

            string rmessage = Option == "VERIFY" ? "Verified" : Option == "UNDO" ? "Undo" : Option == "DEPOSIT" ? "Deposit" : Option == "UNDO_DEPOSIT" ? "Un Deposit" : Option == "CLEAR" ? "Clear" : Option == "UNDO_CLEAR" ? "Undo" : Option == "BOUNCE" ? "Bounce" : Option == "UNDO_BOUNCE" ? "Undo" : "";
            string chequelst = string.Join("/n", datas.AsEnumerable().Select(x => x.ChequeNo).ToList());

            await EventLog("Hire Purchase - Cheque Registry", Option, id, chequelst);
            return Message(rmessage + " Successfully.");
        }

        [ValidateAction(Forms.HirePurchases.ChequeRegistry, Rights.Delete)]
        public async Task<ReturnMessage> DeleteAllCheque([FromForm] List<ChequeList> datas)
        {
            var tableSchemas = new List<SqlMetaData>() {
                  new SqlMetaData("BankID", SqlDbType.BigInt),
                  new SqlMetaData("BankName", SqlDbType.VarChar,100),
                  new SqlMetaData("AccountName", SqlDbType.VarChar,100),
                  new SqlMetaData("AccountNo", SqlDbType.VarChar,50),
                  new SqlMetaData("ChequeNo", SqlDbType.VarChar,50),
                  new SqlMetaData("Amount", SqlDbType.Money),
                  new SqlMetaData("ID", SqlDbType.BigInt),
                  new SqlMetaData("Currency", SqlDbType.VarChar,50)
        }.ToArray();

            var sqlDataRecords = new List<SqlDataRecord>();
            datas.ForEach(x =>
            {
                SqlDataRecord sqlDataRecord = new SqlDataRecord(tableSchemas);
                sqlDataRecord.SetInt64(0, (long)0);
                sqlDataRecord.SetString(1, (string)"");
                sqlDataRecord.SetString(2, (string)"");
                sqlDataRecord.SetString(3, (string)"");
                sqlDataRecord.SetString(4, x.ChequeNo);
                sqlDataRecord.SetDecimal(5, (decimal)0.0);
                sqlDataRecord.SetInt64(6, (long)x.ID);
                sqlDataRecord.SetString(7, (string)"");
                sqlDataRecords.Add(sqlDataRecord);
            });

            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                new SqlParameter("@Option", "VAL_DELETE_ALL"),
                        new SqlParameter
                        {
                        ParameterName = "@ChequeList",
                        Value = sqlDataRecords,
                        SqlDbType = SqlDbType.Structured,
                        TypeName = "dbo.ChequeList"
                        }
            };
            var errors = await _Webcontext.ValidateSqlAsync("spHP_ChequeList", sqlParams);
            if (errors.Any())
                return DeleteError(errors);

            sqlParams[0].Value = "DELETE_ALL";

            long id = await _Webcontext.ExecuteSqlNonQueryAsync("spHP_ChequeList", sqlParams);
            if (id <= 0)
                return Error();

            string chequelst = string.Join("/n", datas.AsEnumerable().Select(x => x.ChequeNo).ToList());
            await EventLog("Hire Purchase - Cheque Registry", "DELETE_ALL", id, chequelst);
            return Message("Cheque deleted.");
        }
        [HttpPost]
        [ValidateAction(Forms.HirePurchases.ChequeRegistry, Rights.Delete)]
        public async Task<ReturnMessage> DeleteCheque([FromForm] long ID, [FromForm] string ChequeNo, [FromForm] long CustomerID)
        {
            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "VAL_DELETE"),
                        new SqlParameter("@ID", ID),
                        new SqlParameter("@ChequeNo", ChequeNo),
                        new SqlParameter("@CustomerID", CustomerID)
            };

            var errors = await _Webcontext.ValidateSqlAsync("spHP_ChequeList", sqlParams);
            if (errors.Any())
                return DeleteError(errors);

            sqlParams[0].Value = "DELETE";

            long id = await _Webcontext.ExecuteSqlNonQueryAsync("spHP_ChequeList", sqlParams);
            if (id <= 0)
                return Error();

            await EventLog("Hire Purchase - Cheque Registry", "DELETE", ID, ChequeNo);
            return Message("Cheque deleted.");
        }
        [HttpPost]
        public async Task<List<DropDownModel>> ReadBank()
        {
            return await _Webcontext.Banks.Select(x => new DropDownModel { Id = x.ID, Text = x.BankName }).ToListAsync();
        }
        [HttpPost]
        public async Task<List<StringTypeModel>> ReadCurrency()
        {
            return await _Webcontext.Currencies.Select(x => new StringTypeModel { Value = x.Currency }).ToListAsync();
        }
        [HttpPost]
        [ValidateAction(Forms.HirePurchases.ChequeRegistry, Rights.Modify)]
        public async Task<ReturnMessage> SaveChequeChangeCustomer([FromForm] string OldCustomerName, [FromForm] string CChequeNo, [FromForm] string NewCustomerName, [FromForm] long NewCustomerID, [FromForm] long ID)
        {
            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "UPDATE_CUSTOMER"),
                        new SqlParameter("@CustomerID", NewCustomerID),
                        new SqlParameter("@ID", ID),
                    };

            long id = await _Webcontext.ExecuteSqlNonQueryAsync("spHP_ChequeList", sqlParams);
            if (id <= 0)
                return Error();

            await EventLog("Hire Purchase - Cheque Registry", "CUSTOMER_CHANGE", OldCustomerName, CChequeNo, NewCustomerName);
            return Message("Customer Updated successfully.");
        }
        [HttpPost]
        [ValidateAction(Forms.HirePurchases.ChequeRegistry, Rights.Modify)]
        public async Task<ReturnMessage> MarkAsBounceDate([FromForm] long ID, [FromForm] string ChequeNo, [FromForm] string CustomerName, [FromForm] DateTime BounceDate, [FromForm] string BounceChequeReason)
        {
            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "CHEQUE_UPDATE_BOUNCE_DATE"),
                        new SqlParameter("@ID", ID),
                        new SqlParameter("@EntryBy", _appUser.CommonEmpNo),
                        new SqlParameter("@BouncedOn", BounceDate),
                        new SqlParameter("@BouncedReason", BounceChequeReason)
                    };

            var errors = await _Webcontext.ValidateSqlAsync("spHP_ChequeList", sqlParams);
            if (errors.Any())
                return DeleteError(errors);

            sqlParams[0].Value = "UPDATE_BOUNCE_DATE";

            long id = await _Webcontext.ExecuteSqlNonQueryAsync("spHP_ChequeList", sqlParams);
            if (id <= 0)
                return Error();

            await EventLog("Hire Purchase - Cheque Registry", "CHEQUE_BOUNCE", CustomerName, ChequeNo);
            return Message("Updated successfully.");
        }
        [HttpPost]
        [ValidateAction(Forms.HirePurchases.ChequeRegistry, Rights.Modify)]
        public async Task<ReturnMessage> RevertAsBounceDate([FromForm] long ID, [FromForm] string ChequeNo, [FromForm] string CustomerName)
        {
            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "REVERT_BOUNCE_DATE"),
                        new SqlParameter("@ID", ID),
                        new SqlParameter("@EntryBy", _appUser.CommonEmpNo)
                    };

            long id = await _Webcontext.ExecuteSqlNonQueryAsync("spHP_ChequeList", sqlParams);
            if (id <= 0)
                return Error();

            await EventLog("Hire Purchase - Cheque Registry", "CHEQUE_BOUNCE", CustomerName, ChequeNo);
            return Message("Reverted successfully.");
        }

        [HttpPost]
        public async Task<List<string>> ReadAccountNo([FromForm] long BankID)
        {
            return await _Webcontext.CompanyAccounts.Where(x => x.BankID == BankID).Select(x => x.CompanyAccountNo).Distinct().ToListAsync();
        }
        [HttpPost]
        public async Task<List<string>> ReadDepositedBy()
        {
            return await _Webcontext.Employees.Select(x => x.ShortName + "-" + x.EmpID).Distinct().ToListAsync();
        }
        [HttpPost]
        public async Task<List<string>> ReadBouncedReason()
        {
            return await _Webcontext.BounceChequeReasons.Select(x => x.Reason).Distinct().ToListAsync();
        }
        #region Deposit Date Change
        [HttpPost]
        [ValidateAction(Forms.HirePurchases.DepositDateChange, Rights.Modify)]
        public async Task<ReturnMessage> SaveDepositDateChange([FromForm] string CustomerName, [FromForm] long ChequeID, [FromForm] string ChequeNo, [FromForm] string ReasonForDelay, [FromForm] DateTime NewDepositDate, [FromForm] DateTime OldDepositDate)
        {
            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "VAL_UPDATE"),
                        new SqlParameter("@ChequeID", ChequeID),
                        new SqlParameter("@NewDepositDate", NewDepositDate),
                        new SqlParameter("@ReasonForDelay", ReasonForDelay),
                        new SqlParameter("@OldDepositDate", OldDepositDate),
                        new SqlParameter("@EntryBy", _appUser.CommonEmpNo)
                    };

            var errors = await _Webcontext.ValidateSqlAsync("sp_DepositDateChange", sqlParams);
            if (errors.Any())
                return Error(errors);

            sqlParams[0].Value = "UPDATE_DEPOSIT_DATE_CHANGE";
            long id = await _Webcontext.ExecuteSqlNonQueryAsync("sp_DepositDateChange", sqlParams);
            if (id <= 0)
                return Error();

            await EventLog("Hire Purchase - Cheque Registry", "DEPOSIT_DATE_CHANGE", CustomerName, ChequeNo, ReasonForDelay, NewDepositDate, OldDepositDate);
            return Message("Deposit Date Updated successfully.");
        }

        [HttpPost]
        [ValidateAction(Forms.HirePurchases.DepositDateChange, Rights.Delete)]
        public async Task<ReturnMessage> DeleteDepositDateChange([FromForm] long ID, [FromForm] string ChequeNo, [FromForm] DateTime NewDepositDate, [FromForm] DateTime OldDepositDate, [FromForm] string CustomerName)
        {
            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "VAL_DELETE"),
                        new SqlParameter("@ID", ID)
            };

            var errors = await _Webcontext.ValidateSqlAsync("sp_DepositDateChange", sqlParams);
            if (errors.Any())
                return DeleteError(errors);

            sqlParams[0].Value = "DELETE";

            long id = await _Webcontext.ExecuteSqlNonQueryAsync("sp_DepositDateChange", sqlParams);
            if (id <= 0)
                return Error();

            await EventLog("Hire Purchase - Cheque Registry", "DELETE", CustomerName, ChequeNo, NewDepositDate, OldDepositDate);
            return Message("Cheque deleted.");
        }

        [HttpPost]
        [ValidateAction(Forms.HirePurchases.DepositDateChange, Rights.View)]
        public async Task<DataSourceResult> ReadDepositDateChange([DataSourceRequest] DataSourceRequest request, [FromForm] int StatusFilter)
        {
            var result = await _Webcontext.ExecuteSpAsync<ChequeDepositDateChangeModel>("sp_DepositDateChange", new { Option = "SELECT", Filter = StatusFilter });
            return await result.ToDataSourceResultAsync(request);
        }

        [HttpPost]
        [ValidateAction(Forms.HirePurchases.DepositDateChange, Rights.Approve)]
        public async Task<ReturnMessage> ApproveDepositDateChange([FromForm] List<ChequeDepositDateChangeModel> datas)
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
                        new SqlParameter("@Option", "VAL_APPROVE_ALL"),
                        new SqlParameter("@EntryBy", _appUser.CommonEmpNo),
                        new SqlParameter
                        {
                            ParameterName = "@Number",
                            Value = sqlDataRecords,
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "dbo.NumberType"
                        }
                    };

            var errors = await _Webcontext.ValidateSqlAsync("sp_DepositDateChange", sqlParams);
            if (errors.Any())
                return ApproveError(errors);

            List<SqlParameter> sqlParameter = new List<SqlParameter>();
            sqlParameter.AddRange(sqlParams.Select(x => ((ICloneable)x).Clone() as SqlParameter));
            sqlParameter[0].Value = "APPROVE_ALL";

            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("sp_DepositDateChange", sqlParameter);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            var chequelst = string.Join('/', datas.Select(x => x.Cust_Name_EN + "_" + x.ChequeNo));
            await EventLog("Hire Purchase - Deposit Date Change", "APPROVE", chequelst);
            return Message("Approved successfully.");
        }
        [HttpPost]
        [ValidateAction(Forms.HirePurchases.DepositDateChange, Rights.Approve)]
        public async Task<ReturnMessage> UndoDepositDateChange([FromForm] List<ChequeDepositDateChangeModel> datas)
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
                        new SqlParameter("@Option", "VAL_UNDO_ALL"),
                        new SqlParameter("@EntryBy", _appUser.CommonEmpNo),
                        new SqlParameter
                        {
                            ParameterName = "@Number",
                            Value = sqlDataRecords,
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "dbo.NumberType"
                        }
                    };

            var errors = await _Webcontext.ValidateSqlAsync("sp_DepositDateChange", sqlParams);
            if (errors.Any())
                return ApproveError(errors);

            List<SqlParameter> sqlParameter = new List<SqlParameter>();
            sqlParameter.AddRange(sqlParams.Select(x => ((ICloneable)x).Clone() as SqlParameter));
            sqlParameter[0].Value = "UNDO";

            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("sp_DepositDateChange", sqlParameter);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            var chequelst = string.Join('/', datas.Select(x => x.Cust_Name_EN + "_" + x.ChequeNo));
            await EventLog("Hire Purchase - Deposit Date Change", "UNDO", chequelst);
            return Message("Undo successfully.");
        }
        #endregion

        #region Cheque Locker Setting
        [ValidateAction(Forms.HirePurchases.ChequeLocker, Rights.View)]
        public async Task<DataSourceResult> ReadLockerList([DataSourceRequest] DataSourceRequest request)
        {
            var result = await (from b in _Webcontext.LockerSettings
                                select new LockerSetting
                                {
                                    ID = b.ID,
                                    LockerName = b.LockerName,
                                    RackNo = b.RackNo,
                                    EntryBy = b.EntryBy,
                                    EntryDate = b.EntryDate,
                                    NoofBin = b.NoofBin,
                                    NoofLevel = b.NoofLevel
                                }).ToListAsync();

            return await result.ToDataSourceResultAsync(request);
        }

        [ValidateAction(Forms.HirePurchases.ChequeLocker, Rights.Modify)]
        public async Task<DataSourceResult> UpdateLockerSetting([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "models")] IEnumerable<LockerSetting> datas)
        {
            foreach (var item in datas)
            {
                if (!await item.SaveAsync())
                    ModelState.AddModelError(item.ErrorMessage);
            }
            return datas.ToDataSourceResult(request, ModelState);
        }

        [ValidateAction(Forms.HirePurchases.ChequeLocker, Rights.Delete)]
        public async Task<ReturnMessage> DeleteLocker([FromForm] LockerSetting data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Bank details deleted.");
        }
        #endregion
        #region Update Cheque Locker Details
        [HttpPost]
        public async Task<List<DropDownModel>> GetLockerName()
        {
            return await _Webcontext.LockerSettings.Select(x => new DropDownModel { Id = x.ID, Text = x.LockerName }).ToListAsync();
        }
        [HttpPost]
        public async Task<List<string>> GetEmployee()
        {
            return await _Webcontext.Employees.Select(x => x.ShortName + '-' + x.EmpID).ToListAsync();
        }
        [HttpPost]
        public async Task<List<int>> GetRackNo([FromForm] long ID)
        {
            int rackNo = await (from l in _Webcontext.LockerSettings
                                where l.ID == ID
                                select l.RackNo).FirstOrDefaultAsync();

            return Enumerable.Range(1, rackNo).ToList();
        }
        [HttpPost]
        public async Task<List<int>> GetBinNo([FromForm] long ID)
        {
            int binNo = await (from l in _Webcontext.LockerSettings
                               where l.ID == ID
                               select l.NoofBin).FirstOrDefaultAsync();

            return Enumerable.Range(1, binNo).ToList();
        }
        [HttpPost]
        public async Task<List<int>> GetLevel([FromForm] long ID)
        {
            int levelNo = await (from l in _Webcontext.LockerSettings
                                 where l.ID == ID
                                 select l.NoofLevel).FirstOrDefaultAsync();

            return Enumerable.Range(1, levelNo).ToList();
        }
        [HttpPost]
        [ValidateAction(Forms.HirePurchases.ChequeLocker, Rights.Modify)]
        public async Task<ReturnMessage> SaveChequeLockerDetails([FromForm] ChequeLockerDetail data, [FromForm] List<ChequeLockerDetail> datas, [FromForm] long StatusFilter)
        {
            var tableSchemas = new List<SqlMetaData>() {
                  new SqlMetaData("Number", SqlDbType.BigInt)
            }.ToArray();

            var sqlDataRecords = new List<SqlDataRecord>();
            sqlDataRecords = new List<SqlDataRecord>();
            datas.ForEach(x =>
            {
                SqlDataRecord sqlDataRecord = new SqlDataRecord(tableSchemas);
                sqlDataRecord.SetInt64(0, (long)x.ID);
                sqlDataRecords.Add(sqlDataRecord);
            });

            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "VAL_SAVE"),
                        new SqlParameter("@OldLockerID", data.OldLockerID),
                        new SqlParameter("@OldRackNo", data.OldRackNo),
                        new SqlParameter("@OldBinNo", data.OldBinNo),
                        new SqlParameter("@OldLevel", data.OldLevelNo),
                        new SqlParameter("@LockerID", data.LockerID),
                        new SqlParameter("@RackNo", data.RackNo),
                        new SqlParameter("@BinNo", data.BinNo),
                        new SqlParameter("@Level", data.LevelNo),
                        new SqlParameter("@ReceivedBy", data.ReceivedBy),
                        new SqlParameter("@ReceivedOn", data.ReceivedOn),
                        new SqlParameter("@ChequeStatus", data.ChequeStatus),
                        new SqlParameter("@StatusUpdatedOn", data.StatusUpdatedOn),
                        new SqlParameter("@EntryBy", _appUser.CommonEmpNo),
                        new SqlParameter("@CustomerID", data.CustomerID),
                        new SqlParameter("@AgreementID", data.AgreementID),
                        new SqlParameter("@PickedBy", data.PickedBy),
                        new SqlParameter("@Remarks", data.Remarks),
                        new SqlParameter("@StatusFilter", StatusFilter),
                        new SqlParameter
                        {
                            ParameterName = "@Number",
                            Value = sqlDataRecords,
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "dbo.NumberType"
                        }
                    };

            var errors = await _Webcontext.ValidateSqlAsync("sp_ChequeLocker", sqlParams);
            if (errors.Any())
                return DeleteError(errors);

            List<SqlParameter> sqlParameter = new List<SqlParameter>();
            sqlParameter.AddRange(sqlParams.Select(x => ((ICloneable)x).Clone() as SqlParameter));
            sqlParameter[0].Value = "UPDATE";

            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("sp_ChequeLocker", sqlParameter);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            var chequelst = string.Join('/', datas.Select(x => x.ChequeNo));
            await EventLog("Hire Purchase - Cheque Safety Locker", "SAVE", chequelst, data.CustomerName, data.AgreementNo, data.OldLockerName, data.OldRackNo, data.OldBinNo, data.OldLevelNo, data.NewLockerName, data.RackNo, data.BinNo, data.LevelNo, data.ChequeStatus);
            return Message("Saved successfully.");
        }

        [HttpPost]
        [ValidateAction(Forms.HirePurchases.ChequeLocker, Rights.Delete)]
        public async Task<ReturnMessage> DeleteChequeLocker([FromForm] long ID, [FromForm] string ChequeNo, [FromForm] int StatusFilter, [FromForm] string CustomerName)
        {
            List<SqlParameter> sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@Option", "VAL_DELETE"),
                        new SqlParameter("@ID", ID),
                        new SqlParameter("@StatusFilter", StatusFilter),
            };

            var errors = await _Webcontext.ValidateSqlAsync("sp_ChequeLocker", sqlParams);
            if (errors.Any())
                return DeleteError(errors);

            sqlParams[0].Value = "DELETE";

            long id = await _Webcontext.ExecuteSqlNonQueryAsync("sp_ChequeLocker", sqlParams);
            if (id <= 0)
                return Error();

            await EventLog("Hire Purchase - Cheque Safety Locker", "DELETE", CustomerName, ChequeNo);
            return Message("Deleted successfully..");
        }
        [HttpPost]
        [ValidateAction(Forms.HirePurchases.ChequeLocker, Rights.Delete)]
        public async Task<ReturnMessage> DeleteAllChequeLocker([FromForm] List<ChequeLockerDetail> datas, [FromForm] int StatusFilter)
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
                        new SqlParameter("@Option", "VAL_SAVE"),
                        new SqlParameter("@StatusFilter", StatusFilter),
                        new SqlParameter
                        {
                            ParameterName = "@Number",
                            Value = sqlDataRecords,
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "dbo.NumberType"
                        }
                    };

            var errors = await _Webcontext.ValidateSqlAsync("sp_ChequeLocker", sqlParams);
            if (errors.Any())
                return DeleteError(errors);

            List<SqlParameter> sqlParameter = new List<SqlParameter>();
            sqlParameter.AddRange(sqlParams.Select(x => ((ICloneable)x).Clone() as SqlParameter));
            sqlParameter[0].Value = "DELETE_ALL";

            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("sp_ChequeLocker", sqlParameter);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            var chequelst = string.Join('/', datas.Select(x => x.ChequeNo + "-" + x.CustomerName));
            await EventLog("Hire Purchase - Cheque Safety Locker", "SAVE", chequelst);
            return Message("Deleted successfully.");
        }
        [HttpPost]
        [ValidateAction(Forms.HirePurchases.ChequeLocker, Rights.View)]
        public async Task<DataSourceResult> ReadChequeSafetyLockerList([DataSourceRequest] DataSourceRequest request, [FromForm] int StatusFilter)
        {
            var result = await _Webcontext.ExecuteSpAsync<ChequeLockerDetailModel>("sp_ChequeLocker", new { Option = "SELECT", StatusFilter = StatusFilter });
            return await result.ToDataSourceResultAsync(request);
        }
        [HttpPost]
        [ValidateAction(Forms.HirePurchases.ChequeLocker, Rights.Modify)]
        public async Task<ReturnMessage> UpdateUnPickedCheques([FromForm] List<ChequeLockerDetail> datas)
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
                        new SqlParameter("@Option", "UN_PICKED"),
                        new SqlParameter
                        {
                            ParameterName = "@Number",
                            Value = sqlDataRecords,
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "dbo.NumberType"
                        }
                    };

            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("sp_ChequeLocker", sqlParams);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            var chequelst = string.Join('/', datas.Select(x => x.ChequeNo + "-" + x.CustomerName));
            await EventLog("Hire Purchase - Cheque Safety Locker", "SAVE", chequelst);
            return Message("Deleted successfully.");
        }
        #endregion

        #region Cheque HandedOver
        [HttpPost]
        [ValidateAction(Forms.HirePurchases.ChequeRegistry, Rights.Modify)]
        public async Task<ReturnMessage> UpdateHandOverCheques([FromForm] List<ChequeHanedOverModel> datas, [FromForm] string EmployeeId)
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
                        new SqlParameter("@Option", "VAL_UPDATE"),
                        new SqlParameter("@HandOverBy", EmployeeId),
                        new SqlParameter
                        {
                            ParameterName = "@Number",
                            Value = sqlDataRecords,
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "dbo.NumberType"
                        }
                    };

            var errors = await _Webcontext.ValidateSqlAsync("spHP_ChequeHandOver", sqlParams);
            if (errors.Any())
                return ApproveError(errors);

            List<SqlParameter> sqlParameter = new List<SqlParameter>();
            sqlParameter.AddRange(sqlParams.Select(x => ((ICloneable)x).Clone() as SqlParameter));
            sqlParameter[0].Value = "UPDATE_HANDOVER";

            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("spHP_ChequeHandOver", sqlParameter);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            var chequelst = string.Join('/', datas.Select(x => x.CustomerName + "_" + x.AgrementRef + "_" + x.ChequeNo));
            await EventLog("Hire Purchase - Cheque HandedOver", "UPDATE", chequelst);
            return Message("Updated successfully.");
        }
        [HttpPost]
        [ValidateAction(Forms.HirePurchases.ChequeRegistry, Rights.Modify)]
        public async Task<ReturnMessage> UndoHandOverCheques([FromForm] List<ChequeHanedOverModel> datas)
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
                        new SqlParameter("@Option", "UNDO_HANDOVER"),
                        new SqlParameter
                        {
                            ParameterName = "@Number",
                            Value = sqlDataRecords,
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "dbo.NumberType"
                        }
                    };

            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("spHP_ChequeHandOver", sqlParams);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            var chequelst = string.Join('/', datas.Select(x => x.CustomerName + "_" + x.AgrementRef + "_" + x.ChequeNo));
            await EventLog("Hire Purchase - Cheque HandedOver", "UNDO", chequelst);
            return Message("Undo successfully.");
        }
        [HttpPost]
        [ValidateAction(Forms.HirePurchases.ChequeRegistry, Rights.View)]
        public async Task<DataSourceResult> ReadHandOverChequeList([DataSourceRequest] DataSourceRequest request, [FromForm] int StatusFilter)
        {
            var result = await _Webcontext.ExecuteSpAsync<ChequeHanedOverModel>("spHP_ChequeHandOver", new { Option = "GET_LIST", StatusFilter });
            return await result.ToDataSourceResultAsync(request);
        }
        #endregion

        #region HP Monthly Status
        [HttpPost]
        [ValidateAction(Forms.HirePurchases.HPMonthlyStatus, Rights.View)]
        public async Task<DataSourceResult> ReadHPMonthlyStatus([DataSourceRequest] DataSourceRequest request, [FromForm] DateTime FromMonth, [FromForm] DateTime ToMonth, [FromForm] string Product = null, [FromForm] string CustomerId = null, [FromForm] bool Proforma=false, [FromForm] bool AdvanceBooking = false, [FromForm] bool CreditEvaluation = false, [FromForm] bool Agreement = false, [FromForm] bool Sales = false, [FromForm] DateTime? Month=null, [FromForm] DateTime? Year = null)
        {
            FromMonth = FromMonth.ToMonthStart();
            var result = await _context.ExecuteSpAsync<MonthlyHPStatusModel>("spPOS_HPStatus", new { FromMonth, ToMonth, Product, CustomerId, Proforma, AdvanceBooking , CreditEvaluation , Sales, Agreement ,Month,Year});
            return await result.ToDataSourceResultAsync(request);
        }
        
        #endregion
    }
}
