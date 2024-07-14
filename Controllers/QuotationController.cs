using BSOL.Core.Entities;
using BSOL.Core;
using BSOL.Helpers;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.Extensions;
using BSOL.Core.Models.Common;
using BSOL.Services;
using BSOL.Core.Helpers;
using BSOL.Core.Models.Retail;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient.Server;
using Microsoft.Data.SqlClient;
using BSOL.Core.Models.Procurement;

namespace BSOL.Controllers
{
    public class QuotationController : BaseController
    {
        //private readonly ICommonNotification _notification;
        private readonly ICommonHelper _commonHelper;
        private readonly NotificationService _notificationService;
        //private readonly IWebHostEnvironment _webHostEnvironment;

        public QuotationController(BSOLContext context, BSOLWebContext Webcontext, AppUser appUser, ICommonHelper commonHelper, NotificationService notificationService) : base(context, Webcontext, appUser)
        {
            _commonHelper = commonHelper;
            _notificationService = notificationService;
        }

        [ValidateAction(Forms.Procurement.Quotations, Rights.View)]
        public async Task<DataSourceResult> ReadQuotations([DataSourceRequest] DataSourceRequest request, [FromQuery] int statusFilter = 0)
        {
            var result = (from q in _Webcontext.Quotations
            join s in _Webcontext.Shops on q.ShopId equals s.Id
                          let f = _Webcontext.FollowupNotifications.Where(x => x.QuotationId == q.Id && x.EmployeeId == _appUser.EmployeeId).Count()
                          let ff = (from fn in _Webcontext.FollowupNotifications
                                    join e in _Webcontext.Employees on fn.EmployeeId equals e.ID
                                    where fn.QuotationId == q.Id
                                    select string.IsNullOrEmpty(e.ImagePath) ? e.FullName : string.Format("{0}-{1}", e.ImagePath, e.FullName)
                                    ).ToList()
                          let fc = _Webcontext.FollowUps.Where(x => x.ReferenceId == q.Id && x.Reference == "Quotation").Count()
                          where s.CompanyId == _appUser.CompanyId && q.Active
                          && (statusFilter == 0 ? !q.ApprovedDate1.HasValue
                             : statusFilter == 1 ? q.ApprovedDate1.HasValue && !q.ClosedDate.HasValue
                             : q.ClosedDate.HasValue)
                          select new Quotation
                          {
                              Id = q.Id,
                              ShopId = q.ShopId,
                              ShopCode = q.ShopCode,
                              RefNo = q.RefNo,
                              QuotationType = q.QuotationType,
                              QuotationDate = q.QuotationDate,
                              CustomerId = q.CustomerId,
                              CustomerType = q.CustomerType,
                              CustomerName = q.CustomerName,
                              ContactName = q.ContactName,
                              ContactNo = q.ContactNo,
                              CustomerRefNo = q.CustomerRefNo,
                              Currency = q.Currency,
                              ConversionRate = q.ConversionRate,
                              DiscountPercent = q.DiscountPercent,
                              DiscountAmount = q.DiscountAmount,
                              GSTPercent = q.GSTPercent,
                              GSTAmount = q.GSTAmount,
                              TotalAmount = q.TotalAmount,
                              Notes = q.Notes,
                              ApprovedBy1 = q.ApprovedBy1,
                              ApprovedDate1 = q.ApprovedDate1,
                              ApprovedBy2 = q.ApprovedBy2,
                              ApprovedDate2 = q.ApprovedDate2,
                              ClosedBy = q.ClosedBy,
                              ClosedDate = q.ClosedDate,
                              ClosedReason = q.ClosedReason,
                              Active = q.Active,
                              CreatedBy = q.CreatedBy,
                              CreatedOn = q.CreatedOn,
                              RefNoFormatted = q.RefNoFormatted,
                              AllowNotification = f > 0,
                              FollowUpCount = fc,
                              ImagePath = string.Join(",", ff)
                          });
            return await result.ToDataSourceResultAsync(request);
        }
        public async Task<DataSourceResult> ReadQuotationItems([DataSourceRequest] DataSourceRequest request, [FromForm] long customerId = 0)
        {
            request.Sorts.SortInterceptor("SKU");

            var result = (from si in _Webcontext.ShopItems
                          join i in _Webcontext.Items on si.ItemId equals i.Id
                          join ic in _Webcontext.ItemCategories on i.ItemCategoryId equals ic.Id
                          join c in _Webcontext.CustomerSellingCodes on new { ItemId = i.Id, CustomerId = customerId }
                                    equals new { c.ItemId, c.CustomerId } into grpC
                          from ci in grpC.DefaultIfEmpty()
                          where si.ShopId == _appUser.ShopId && i.Active && i.VerifiedOn.HasValue
                          select new QuotationItemModel
                          {
                              ItemId = i.Id,
                              SKU = i.SKU,
                              UPC = i.UPC,
                              Description = i.Description,
                              Category = ic.Category,
                              ItemType = i.ItemType,
                              Rate = si.SellingPriceMode == "Percentage" ? i.SellingRate + ((i.SellingRate * si.SellingPrice) / 100) :
                                         si.SellingPriceMode == "Fixed" ? i.SellingRate + si.SellingPrice : si.SellingPriceMode == "Custom" ? si.SellingPrice : i.SellingRate,
                              //Rate = i.SellingRate,
                              ShopStock = si.Stock,
                              MasterStock = i.Stock,
                              IsInventory = i.IsInventory,
                              ItemCode = i.ItemCode,
                              CustomerCode = ci.CustomerItemCode,
                              GSTApplicable = i.GSTApplicable,
                              SKUFormatted = i.SKUFormatted,
                              QuotaionUnits = (from siu in _Webcontext.SellingItemUnits
                                               join uom in _Webcontext.UnitOfMeasures on siu.SellingUnitId equals uom.ID into grpUO
                                               from luom in grpUO.DefaultIfEmpty()
                                               where siu.ItemId == i.Id
                                               select
                                               new SellingUnitModel
                                               {
                                                   Id = siu.Id,
                                                   SellingUnitId = siu.SellingUnitId,
                                                   SellingUnitRate = siu.SellingUnitRate,
                                                   SellingUnit = luom.UOM,
                                                   Conversion = siu.Conversion
                                               }).ToList(),
                              Unit = i.Unit,
                              Conversion = 1,
                              MinSellingRate = si.SellingPriceMode == "Percentage" ? i.SellingRate + ((i.SellingRate * si.SellingPrice) / 100) :
                                             si.SellingPriceMode == "Fixed" ? i.SellingRate + si.SellingPrice : si.SellingPriceMode == "Custom" ? si.SellingPrice : i.SellingRate
                          });
            return await result.ToDataSourceResultAsync(request);
        }

        public async Task<DataSourceResult> ReadQuotationDetails([DataSourceRequest] DataSourceRequest request, [ModelBinder(typeof(IdentityDecryptor))] long quotationId)
        {
            var result = (from id in _Webcontext.QuotationDetails
                          join i in _Webcontext.Quotations on id.QuotationId equals i.Id
                          join im in _Webcontext.Items on id.ItemId equals im.Id
                          join si in _Webcontext.ShopItems on im.Id equals si.ItemId
                          join c in _Webcontext.CustomerSellingCodes on new { ItemId = im.Id, CustomerId = i.CustomerId }
                                    equals new { c.ItemId, c.CustomerId } into grpC
                          from ci in grpC.DefaultIfEmpty()
                          where id.QuotationId == quotationId && si.ShopId == _appUser.ShopId
                          select new QuotationDetail
                          {
                              Id = id.Id,
                              QuotationId = id.QuotationId,
                              ItemId = id.ItemId,
                              Description = id.Description,
                              BaseRate = id.BaseRate,
                              Rate = id.Rate,
                              Qty = id.ActualQty,
                              TotalPrice = id.TotalPrice,
                              SKU = im.SKU,
                              UPC = im.UPC,
                              IsInventory = im.IsInventory,
                              ItemCode = im.ItemCode,
                              NicNo = ci.CustomerItemCode,
                              GSTApplicable = im.GSTApplicable,
                              QuotationUnit = id.QuotationUnit,
                              ActualQty = id.Qty,
                              Conversion = id.Conversion,
                              QuotaionUnits = (from siu in _Webcontext.SellingItemUnits
                                               join uom in _Webcontext.UnitOfMeasures on siu.SellingUnitId equals uom.ID into grpUO
                                               from luom in grpUO.DefaultIfEmpty()
                                               where siu.ItemId == im.Id
                                               select
                                               new SellingUnitModel
                                               {
                                                   Id = siu.Id,
                                                   SellingUnitId = siu.SellingUnitId,
                                                   SellingUnitRate = siu.SellingUnitRate,
                                                   SellingUnit = luom.UOM,
                                                   Conversion = siu.Conversion
                                               }).ToList(),
                              Unit = im.Unit,
                              MinSellingRate = id.MinSellingRate

                          });
            return await result.ToDataSourceResultAsync(request);
        }
        public async Task<QuotationItemModel> GetQuotationItem([FromForm] string searchText, [FromForm] long customerId, [FromForm] long? ItemId = null)
        {
            ItemId = ItemId ?? 0;
            var item = await (from si in _Webcontext.ShopItems
                              join i in _Webcontext.Items on si.ItemId equals i.Id
                              join ic in _Webcontext.ItemCategories on i.ItemCategoryId equals ic.Id
                              join c in _Webcontext.CustomerSellingCodes on new { ItemId = i.Id, CustomerId = customerId }
                                    equals new { c.ItemId, c.CustomerId } into grpC
                              from ci in grpC.DefaultIfEmpty()
                              where i.Active && i.VerifiedOn.HasValue && si.ShopId == _appUser.ShopId &&
                              (((i.SKU.ToString().Contains(searchText)) || (i.UPC.ToString().Contains(searchText))
                              || (i.ItemCode.Contains(searchText)) || (i.Description.Contains(searchText)))
                              && ItemId == 0) || (i.Id == ItemId && si.ItemId == ItemId && si.ShopId == _appUser.ShopId)
                              select new QuotationItemModel
                              {
                                  ItemId = i.Id,
                                  SKU = i.SKU,
                                  UPC = i.UPC,
                                  CustomerCode = ci.CustomerItemCode,
                                  Description = i.Description,
                                  Category = ic.Category,
                                  ItemType = i.ItemType,
                                  Rate = si.SellingPriceMode == "Percentage" ? i.SellingRate + ((i.SellingRate * si.SellingPrice) / 100) :
                                         si.SellingPriceMode == "Fixed" ? i.SellingRate + si.SellingPrice : si.SellingPriceMode == "Custom" ? si.SellingPrice : i.SellingRate,
                                  //Rate = i.SellingRate,
                                  ShopStock = si.Stock,
                                  MasterStock = i.Stock,
                                  IsInventory = i.IsInventory,
                                  GSTApplicable = i.GSTApplicable,
                                  SKUFormatted = i.SKUFormatted,
                                  QuotaionUnits = (from siu in _Webcontext.SellingItemUnits
                                                   join uom in _Webcontext.UnitOfMeasures on siu.SellingUnitId equals uom.ID into grpUO
                                                   from luom in grpUO.DefaultIfEmpty()
                                                   where siu.ItemId == i.Id
                                                   select
                                                   new SellingUnitModel
                                                   {
                                                       Id = siu.Id,
                                                       SellingUnitId = siu.SellingUnitId,
                                                       SellingUnitRate = siu.SellingUnitRate,
                                                       SellingUnit = luom.UOM,
                                                       Conversion = siu.Conversion
                                                   }).ToList(),
                                  Unit = i.Unit,
                                  Conversion = 1,
                                  MinSellingRate = si.SellingPriceMode == "Percentage" ? i.SellingRate + ((i.SellingRate * si.SellingPrice) / 100) :
                                             si.SellingPriceMode == "Fixed" ? i.SellingRate + si.SellingPrice : si.SellingPriceMode == "Custom" ? si.SellingPrice : i.SellingRate

                              }).FirstOrDefaultAsync();

            return item;
        }


        [ValidateAction(Forms.Procurement.Quotations, Rights.Approve)]
        public async Task<ReturnMessage> VerifyQuotation([FromForm] Quotation data)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_Quotation", new { Option = "VAL_VERIFY", data.Id, ApprovedBy = _appUser.CommonEmpNo });
            if (errors.Any())
                return ApproveError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_Quotation", new { Option = "VERIFY", data.Id, ApprovedBy = _appUser.CommonEmpNo });
            if (retVal <= 0)
                return DBError();

            await EventLogAsync("QUOTATION", ActionType.Approve, "Quotations", data.Id, data.RefNoFormatted);
            return Message("Quotation verified.");
        }

        [ValidateAction(Forms.Procurement.Quotations, Rights.Reject)]
        public async Task<ReturnMessage> RevertQuotation([FromForm] Quotation data)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_Quotation", new { Option = "VAL_REVERT", data.Id, ApprovedBy = _appUser.CommonEmpNo });
            if (errors.Any())
                return RejectError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_Quotation", new { Option = "REVERT", data.Id });
            if (retVal <= 0)
                return DBError();

            await EventLogAsync("QUOTATION", ActionType.Reject, "Quotations", data.Id, data.RefNoFormatted);
            return Message("Quotation un-verified.");
        }

        [ValidateAction(Forms.Procurement.Quotations, Rights.Delete)]
        public async Task<ReturnMessage> DeleteQuotation([FromForm] Quotation data)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_Quotation", new { Option = "VAL_DELETE", data.Id });
            if (errors.Any())
                return DeleteError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_Quotation", new { Option = "DELETE", data.Id });
            if (retVal <= 0)
                return DBError();

            //await new Document() { DocumentRoot = _commonHelper.GetDocumentRoot() }.DeleteByReference(DocumentReference.Quotations, data.Id);

            await EventLogAsync("QUOTATION", ActionType.Delete, "Quotations", data.Id, data.RefNoFormatted);
            return Message("Quotation deleted.");
        }
        public async Task<ReturnMessage> CloseQuotation([FromForm] Quotation data)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_Quotation", new { Option = "VAL_CLOSE", data.Id, data.ClosedReason });
            if (errors.Any())
                return SaveError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_Quotation", new { Option = "CLOSE", data.Id, ClosedBy = _appUser.CommonEmpNo, data.ClosedReason });
            if (retVal <= 0)
                return DBError();

            await EventLogAsync("QUOTATION", ActionType.Edit, "Quotations", data.Id, data.RefNoFormatted, "Closed");
            return Message("Quotation marked as Closed.");
        }

        [ValidateAction(Forms.Procurement.Quotations, Rights.Modify)]
        public async Task<ReturnMessage> OpenQuotation([FromForm] Quotation data)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_Quotation", new { Option = "VAL_OPEN", data.Id });
            if (errors.Any())
                return SaveError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_Quotation", new { Option = "OPEN", data.Id });
            if (retVal <= 0)
                return DBError();

            await EventLogAsync("QUOTATION", ActionType.Edit, "Quotations", data.Id, data.RefNoFormatted, "Open");
            return Message("Quotation marked as Open.");
        }
        public async Task<List<string>> ReadClosedReasons()
        {
            var result = await (from q in _Webcontext.Quotations
                                join c in _Webcontext.Customers on q.CustomerId equals c.ID
                                where c.CompanyId == _appUser.CompanyId && q.Active
                                where !string.IsNullOrEmpty(q.ClosedReason)
                                select q.ClosedReason).Distinct().ToListAsync();
            result.AddRange(Utilities.EnumToString<QuotationClosedReason>());
            return result.Distinct().ToList();
        }

        [ValidateAction(Forms.Procurement.Quotations, Rights.Modify)]
        public async Task<Quotation> GetQuotation([ModelBinder(typeof(IdentityDecryptor))] long id)
        {
            return await (from q in _Webcontext.Quotations
                          join lc in _Webcontext.Customers on q.CustomerId equals lc.ID into grpC//TODO:
                          from c in grpC.DefaultIfEmpty()
                          //where q.Id == id && c.Active
                          select new Quotation
                          {
                              Id = q.Id,
                              ShopId = q.ShopId,
                              ShopCode = q.ShopCode,
                              RefNo = q.RefNo,
                              QuotationType = q.QuotationType,
                              QuotationDate = q.QuotationDate,
                              CustomerId = q.CustomerId,
                              CustomerType = q.CustomerType,
                              CustomerName = q.CustomerName,
                              ContactName = q.ContactName,
                              ContactNo = q.ContactNo,
                              CustomerRefNo = q.CustomerRefNo,
                              Currency = q.Currency,
                              ConversionRate = q.ConversionRate,
                              DiscountPercent = q.DiscountPercent,
                              DiscountAmount = q.DiscountAmount,
                              GSTPercent = q.GSTPercent,
                              GSTAmount = q.GSTAmount,
                              TotalAmount = q.TotalAmount,
                              Notes = q.Notes,
                              ApprovedBy1 = q.ApprovedBy1,
                              ApprovedDate1 = q.ApprovedDate1,
                              ApprovedBy2 = q.ApprovedBy2,
                              ApprovedDate2 = q.ApprovedDate2,
                              ClosedBy = q.ClosedBy,
                              ClosedDate = q.ClosedDate,
                              ClosedReason = q.ClosedReason,
                              Active = q.Active,
                              CreatedBy = q.CreatedBy,
                              CreatedOn = q.CreatedOn,
                              RefNoFormatted = q.RefNoFormatted
                          }).FirstOrDefaultAsync();
        }

        //public async Task<object> GetQuotationCustomerDetails([FromForm] long customerId)
        //{
        //    return await (from c in _Webcontext.Customers
        //                  from cc in _Webcontext.CustomerContacts.Where(x => x.CustomerId == customerId && x.IsPrimaryContact).Take(1).DefaultIfEmpty()
        //                  where c.Id == customerId && c.Active
        //                  select new
        //                  {
        //                      c.CustomerType,
        //                      cc.ContactName,
        //                      cc.ContactNo
        //                  }).FirstOrDefaultAsync();
        //}

        [ValidateAction(Forms.Procurement.Quotations, Rights.Modify)]
        public async Task<ReturnMessage> SaveQuotation([FromForm] Quotation quotation, [FromForm] List<QuotationDetail> quotationDetails)
        {
            var tableSchema = new List<SqlMetaData>() {
                new SqlMetaData("Id", SqlDbType.BigInt),
                new SqlMetaData("ItemId", SqlDbType.BigInt),
                new SqlMetaData("Description", SqlDbType.VarChar, -1),
                new SqlMetaData("BaseRate", SqlDbType.Money),
                new SqlMetaData("Rate", SqlDbType.Money),
                new SqlMetaData("Qty", SqlDbType.Decimal,18,2),
                new SqlMetaData("TotalPrice", SqlDbType.Money),
                new SqlMetaData("QuotationUnit", SqlDbType.VarChar,-1),
                new SqlMetaData("Conversion", SqlDbType.Decimal,18,2),
                new SqlMetaData("MinSellingRate", SqlDbType.Decimal,18,2)
            }.ToArray();
            var sqlDataRecords = new List<SqlDataRecord>();
            quotationDetails.ForEach(x =>
            {
                SqlDataRecord sqlDataRecord = new SqlDataRecord(tableSchema);
                sqlDataRecord.SetInt64(0, x.Id);
                sqlDataRecord.SetInt64(1, x.ItemId);
                sqlDataRecord.SetString(2, x.Description ?? "");
                sqlDataRecord.SetDecimal(3, x.BaseRate);
                sqlDataRecord.SetDecimal(4, x.Rate);
                sqlDataRecord.SetDecimal(5, x.Qty);
                sqlDataRecord.SetDecimal(6, x.TotalPrice);
                sqlDataRecord.SetString(7, x.QuotationUnit ?? "");
                sqlDataRecord.SetDecimal(8, x.Conversion);
                sqlDataRecord.SetDecimal(9, x.MinSellingRate);
                sqlDataRecords.Add(sqlDataRecord);
            });

            List<SqlParameter> sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@Option", "VAL_SAVE"),
                new SqlParameter("@Id", quotation.Id),
                new SqlParameter("@ShopId", quotation.ShopId),
                new SqlParameter("@QuotationDate", quotation.QuotationDate),
                new SqlParameter
                {
                    ParameterName = "@QuotationDetails",
                    Value = sqlDataRecords,
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.QuotationDetailType"
                }
            };

            var errors = await _Webcontext.ValidateSqlAsync("spBSOL_Quotation", sqlParams);
            if (errors.Any())
                return SaveError(errors);

            sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@Option", quotation.Id > 0 ? "UPDATE" : "INSERT"),
                new SqlParameter("@Id", quotation.Id),
                new SqlParameter("@ShopId", quotation.ShopId),
                new SqlParameter("@QuotationType", quotation.QuotationType),
                new SqlParameter("@QuotationDate", quotation.QuotationDate),
                new SqlParameter("@CustomerId", quotation.CustomerId),
                new SqlParameter("@CustomerType", quotation.CustomerType),
                new SqlParameter("@CustomerName", quotation.CustomerName),
                new SqlParameter("@ContactName", quotation.ContactName),
                new SqlParameter("@ContactNo", quotation.ContactNo),
                new SqlParameter("@CustomerRefNo", quotation.CustomerRefNo ?? ""),
                new SqlParameter("@Currency", quotation.Currency),
                new SqlParameter("@ConversionRate", quotation.ConversionRate),
                new SqlParameter("@DiscountPercent", quotation.DiscountPercent),
                new SqlParameter("@DiscountAmount", quotation.DiscountAmount),
                new SqlParameter("@GSTPercent", quotation.GSTPercent),
                new SqlParameter("@GSTAmount", quotation.GSTAmount),
                new SqlParameter("@TotalAmount", quotation.TotalAmount),
                new SqlParameter("@Notes", quotation.Notes ?? ""),
                new SqlParameter("@CreatedBy", _appUser.CommonEmpNo),
                new SqlParameter
                {
                    ParameterName = "@QuotationDetails",
                    Value = sqlDataRecords,
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.QuotationDetailType"
                }
            };

            //int retVal = await _context.ExecuteSqlNonQueryAsync("spBSOL_Quotation", sqlParams);
            //if (retVal <= 0)
            //    return DBError();

            long id = await _Webcontext.ExecuteSqlNonQueryWithReturnAsync<long>("spBSOL_Quotation", sqlParams);
            if (id <= 0)
                return DBError();
            if (quotation.Id > 0)
               await EventLogAsync("QUOTATION", ActionType.Edit, "Quotations", quotation.Id, quotation.RefNoFormatted);

                //if (quotation.Id > 0)
                //    await EventLogAsync("QUOTATION", ActionType.Edit, "Quotations", quotation.Id, quotation.RefNoFormatted);
                //else
                //    await _notification.Send(Notification.Quotation.Request, id);
                return Message("Quotation saved.", id);
        }

    }
}
