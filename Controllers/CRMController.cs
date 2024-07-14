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
using Kendo.Mvc.Resources;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Drawing;

namespace BSOL.Controllers
{
    public class CRMController : BaseController
    {
        private readonly ICommonHelper _commonHelper;
        public CRMController(BSOLContext context, BSOLWebContext webContext, AppUser appUser, ICommonHelper commonHelper) : base(context, webContext, appUser)
        {
            _commonHelper = commonHelper;
        }

        #region Customer
        [HttpPost]
        [ValidateAction(Forms.CRM.Customer, Rights.View)]
        public async Task<DataSourceResult> ReadCustomer([DataSourceRequest] DataSourceRequest request, [FromForm] int StatusFilter)
        {
            var result = await (from cd in _Webcontext.Customers
                                join il in _Webcontext.Islands on cd.IslandID equals il.ID into grplis
                                from gis in grplis.DefaultIfEmpty()

                                join at in _Webcontext.Atolls on gis.AtollID equals at.ID into grplat
                                from gat in grplat.DefaultIfEmpty()
                                where cd.Active == true && ((StatusFilter == 0 && cd.ApprovedDate == null) || (StatusFilter == 1 && cd.ApprovedDate != null))
                                select new
                                {
                                    cd.ID,
                                    cd.NicNo,
                                    cd.NameEN,
                                    cd.NameDHI,
                                    cd.PermanentAddressEN,
                                    cd.PermanentAddressDHI,
                                    cd.PresentAddressEN,
                                    cd.PresentAddressDHI,
                                    cd.ContactNo,
                                    cd.MobileNo,
                                    cd.EmailID,
                                    cd.FaxNo,
                                    cd.TINNo,
                                    cd.WorkedMonth,
                                    cd.MonthlyIncome,
                                    cd.CustomerType,
                                    cd.EmployerType,
                                    cd.EmployerName,
                                    cd.EmployerDetails,
                                    cd.EmployerPresentAddressEN,
                                    cd.EmployerPresentAddressDHI,
                                    cd.EmployerContactNo,
                                    cd.EmergencyContactNameEN,
                                    cd.EmergencyContactNameDHI,
                                    cd.EmergencyContactNo,
                                    cd.Remarks,
                                    cd.EntryBy,
                                    cd.EntryDate,
                                    cd.Active,
                                    NicExpiredOn = cd.NicExpiredOn,
                                    DOB = cd.DOB,
                                    cd.Nationality,
                                    cd.Gender,
                                    AtollID = (long?)gis.AtollID,
                                    IslandID = cd.IslandID,
                                    Island = gis.IslandName,
                                    Atoll = gat.AtollName,
                                    IsHPCustomer = (bool?)cd.IsHPCustomer,
                                    cd.ApprovedBy,
                                    cd.ApprovedDate,
                                    RegistrationType = cd.IsHPCustomer == true ? "HP" : "Cheque"
                                }
                               ).ToListAsync();
            return await result.ToDataSourceResultAsync(request);
        }
        [HttpPost]
        [ValidateAction(Forms.CRM.Customer, Rights.Modify)]
        public async Task<ReturnMessage> SaveCustomer([FromForm] Customer data)
        {
            List<SqlParameter> sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@Option", "VAL_SAVE"),
                new SqlParameter("@ID",data.ID),
                new SqlParameter("@NicNo",data.NicNo),
                new SqlParameter("@NameEN",data.NameEN),
                new SqlParameter("@NameDHI",data.NameDHI),
                new SqlParameter("@PermanentAddressEN",data.PermanentAddressEN),
                new SqlParameter("@PermanentAddressDHI",data.PermanentAddressDHI),
                new SqlParameter("@PresentAddressEN",data.PresentAddressEN),
                new SqlParameter("@PresentAddressDHI",data.PresentAddressDHI),
                new SqlParameter("@ContactNo",data.ContactNo),
                new SqlParameter("@MobileNo",data.MobileNo),
                new SqlParameter("@EmailID",data.EmailID),
                new SqlParameter("@FaxNo",data.FaxNo),
                new SqlParameter("@TINNo",data.TINNo),
                new SqlParameter("@WorkedMonth",data.WorkedMonth),
                new SqlParameter("@MonthlyIncome",data.MonthlyIncome),
                new SqlParameter("@CustomerType",data.CustomerType),
                new SqlParameter("@EmployerType",data.EmployerType),
                new SqlParameter("@EmployerName",data.EmployerName),
                new SqlParameter("@EmployerDetails",data.EmployerDetails),
                new SqlParameter("@EmployerPresentAddressEN",data.EmployerPresentAddressEN),
                new SqlParameter("@EmployerPresentAddressDHI",data.EmployerPresentAddressDHI),
                new SqlParameter("@EmployerContactNo",data.EmployerContactNo),
                new SqlParameter("@EmergencyContactNameEN",data.EmergencyContactNameEN),
                new SqlParameter("@EmergencyContactNameDHI",data.EmergencyContactNameDHI),
                new SqlParameter("@EmergencyContactNo",data.EmergencyContactNo),
                new SqlParameter("@Remarks",data.Remarks),
                new SqlParameter("@EntryBy",_appUser.CommonEmpNo),
                new SqlParameter("@DOB",data.DOB),
                new SqlParameter("@Nationality",data.Nationality),
                new SqlParameter("@NicExpiredOn",data.NicExpiredOn),
                new SqlParameter("@Gender",data.Gender),
                new SqlParameter("@Atoll",data.Atoll),
                new SqlParameter("@Island",data.Island),
                new SqlParameter("@IslandID",data.IslandID),
                new SqlParameter("@IsHPCustomer",data.IsHPCustomer)
            };


            var errors = await _Webcontext.ValidateSqlAsync("spFMS_Customer", sqlParams);
            if (errors.Any())
                return Error(errors);

            sqlParams[0].Value = data.ID > 0 ? "UPDATE" : "INSERT";
            long id = await _Webcontext.ExecuteSqlNonQueryAsync("spFMS_Customer", sqlParams);
            if (id <= 0)
                return Error();

            return Message("Customer Details Saved Successfully");
        }
        [ValidateAction(Forms.CRM.Customer, Rights.Delete)]
        public async Task<ReturnMessage> DeleteCustomer([FromForm] Customer data)
        {
            var errors = await _Webcontext.ValidateAsync("spFMS_Customer", new { Option = "VAL_REVERT", data.ID });
            if (errors.Any())
                return DeleteError(errors);

            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Customer details deleted.");
        }
        [HttpPost]
        public async Task<ReturnMessage> CheckNicNo([FromForm] string NicNo)
        {
            var errors = await _Webcontext.ValidateAsync("spFMS_Customer", new { Option = "VAL_NIC_NO", NicNo });
            if (errors.Any())
                return SaveError(errors);
            else
                return Message("Ok");
        }

        [ValidateAction(Forms.CRM.Customer, Rights.Approve)]
        public async Task<ReturnMessage> ApprovedItem([FromForm] Customer data)
        {
            if (!await data.ApproveAsync())
                return ApproveError(data.ErrorMessage);

            return Message("Customer Approved.");
        }
        [ValidateAction(Forms.CRM.Customer, Rights.Reject)]
        public async Task<ReturnMessage> RevertItem([FromForm] Customer data)
        {
            //var errors = await _Webcontext.ValidateAsync("spFMS_Customer", new { Option = "VAL_REVERT", data.ID });
            //if (errors.Any())
            //    return ApproveError(errors);

            if (!await data.RejectAsync())
                return UndoError(data.ErrorMessage);

            return Message("Reverted Successfully.");
        }

        #endregion
    }
}
