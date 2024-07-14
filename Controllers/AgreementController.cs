using BSOL.Core;
using BSOL.Helpers;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOL.Core.Models.HirePurchase;
using Kendo.Mvc.Extensions;
using BSOL.Core.Models.Common;

namespace BSOL.Controllers
{
    public class AgreementController : BaseController
    {
        public AgreementController(BSOLContext context, AppUser appUser) : base(context, appUser)
        {
        }

        [HttpPost]
        [ValidateAction(Forms.HirePurchases.Agreement, Rights.View)]
        public async Task<DataSourceResult> ReadAgreement([DataSourceRequest] DataSourceRequest Request, [FromForm] int Status)
        {
            var result = (from AG in _context.Agreements
                          join WP in _context.WebProformas on AG.Proforma_No equals WP.RefNo
                          join CR in _context.CreditEvaluations on WP.RefNo equals CR.BookingId
                          join CD in _context.CustomerDetails on CR.CustomerRef equals CD.Customer_Id
                          join AB in _context.AdvanceBookings on CR.BookingId equals AB.ProInv_No

                          where AG.Cancel_flag == false && WP.CancelFlag == false && CR.CancelFlag == false && CD.Cancel_Flag == false && AB.CancelFlag == false
                          && ((Status == 0 && !_context.BillingMasters.Any(x => x.ProInv_No == AG.Proforma_No && !x.Cancel_Flag))
                             || (Status == 1 && _context.BillingMasters.Any(x => x.ProInv_No == AG.Proforma_No && !x.Cancel_Flag))
                             )
                          orderby AG.tbl_Id descending
                          select new AgreementModel
                          {
                              tbl_Id = AG.tbl_Id,
                              ProRefNo = AG.Proforma_No,
                              Cust_Name_EN = CD.Cust_Name_EN,
                              Product = WP.Product,
                              ItemCode = WP.ItemCode,
                              BinnedDate = AG.BinnedDate,
                              PickedDate = AG.PickedDate,
                              LockerStatus = AG.LockerStatus,
                              AgreementType = AG.AgreementType,
                              Agrement_Date = AG.Agrement_Date,
                              InstallmentStart = AG.InstallmentStart,
                              Customer_Type = WP.CustomerType,
                              Cust_Ref = CD.Cust_Ref,
                              AgrementRef = AG.AgrementRef,
                              Guarantor_ID = CR.Guarantor_ID,
                              Guarantor_Name = CR.Guarantor_Name,
                              EntryBy = AG.EntryBy
                          });

            return await result.ToDataSourceResultAsync(Request);
        }

        [HttpPost]
        [ValidateAction(Forms.HirePurchases.Agreement, Rights.View)]
        public async Task<DataSourceResult> ReadAgreementList([DataSourceRequest] DataSourceRequest Request, [FromForm] long AgreementID)
        {
            List<AgreementListModel> agreementListDetails = await _context.ExecuteSpAsync<AgreementListModel>("spHP_AgreementDetails", new { Options = "AGREEMENT_LIST", tbl_Id = AgreementID });
            return agreementListDetails.ToDataSourceResult(Request);
        }

        [HttpPost]
        [ValidateAction(Forms.HirePurchases.Agreement, Rights.View)]
        public async Task<List<DropDownModel>> ReadSigningEmployee()
        {
            List<DropDownModel> result = await _context.ExecuteSpAsync<DropDownModel>("spHP_AgreementDetails", new { Options = "GET_SIGNEDNAME" });
            return result;
        }
    }
}


