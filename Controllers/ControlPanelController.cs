using BSOL.Core;
using BSOL.Core.Entities;
using BSOL.Core.Models.Common;
using BSOL.Helpers;
using DocumentFormat.OpenXml.Wordprocessing;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace BSOL.Controllers
{
    public class ControlPanelController : BaseController
    {
        private readonly ICommonHelper _commonHelper;
        public ControlPanelController(BSOLContext context, BSOLWebContext Webcontext, AppUser appUser, ICommonHelper commonHelper) : base(context, Webcontext, appUser)
        {
            _commonHelper = commonHelper;
        }

        #region Port settings
        [ValidateAction(Forms.Procurement.ControlPanel, Rights.View)]
        public async Task<DataSourceResult> ReadPortSettings([DataSourceRequest] DataSourceRequest request)
        {
            var result = _Webcontext.Ports.Where(x => x.CompanyId == _appUser.CompanyId).ToList();
            return await result.ToDataSourceResultAsync(request);
        }
        [ValidateAction(Forms.Procurement.ControlPanel, Rights.Modify)]
        public async Task<DataSourceResult> SavePortSettings([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "Models")] IEnumerable<Port> datas)
        {
            foreach (var port in datas)
            {
                port.CompanyId = _appUser.CompanyId;
                if (!await port.SaveAsync())
                    ModelState.AddModelError(port.ErrorMessage);
            }
            return datas.ToDataSourceResult(request, ModelState);
        }
        [ValidateAction(Forms.Procurement.ControlPanel, Rights.Delete)]
        public async Task<ReturnMessage> DeletePortSetting([FromForm] Port data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Port setting deleted successfully.");
        }
        #endregion

        #region ShipmentExpenseSetting
        [ValidateAction(Forms.Procurement.ControlPanel, Rights.View)]
        public async Task<DataSourceResult> ReadShipmentSettings([DataSourceRequest] DataSourceRequest request)
        {
            var result = (from SPC in _Webcontext.ShipmentExpenseCategories
                          join Ic in _Webcontext.ItemCategories on SPC.ItemCategoryId equals Ic.Id
                          select new ShipmentExpenseCategory
                          {
                              Id = SPC.Id,
                              ShipmentType = SPC.ShipmentType,
                              Category = Ic.Category,
                              ExpenseCategory = SPC.ExpenseCategory,
                              ExpenseCategoryDetails = SPC.ExpenseCategoryDetails,
                              Active = SPC.Active
                          });
            return await result.ToDataSourceResultAsync(request);

        }
        public async Task<List<DropDownModel>> ReadCategory()
        {
            return await _Webcontext.ItemCategories.Select(x => new DropDownModel { Id = x.Id, Text = x.Category }).ToListAsync();
        }
        [ValidateAction(Forms.Procurement.ControlPanel, Rights.Modify)]
        public async Task<DataSourceResult> SaveShipmentSettings([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "Models")] IEnumerable<ShipmentExpenseCategory> datas)
        {
            var existingList = await (from ec in _Webcontext.ShipmentExpenseCategories
                                      select ec
                                     ).ToListAsync();

            var lexistingRecords = (from ec in existingList
                                    join dt in datas on new { ec.ShipmentType, ec.ItemCategoryId, ec.ExpenseCategory, ec.ExpenseCategoryDetails } equals new { dt.ShipmentType, dt.ItemCategoryId, dt.ExpenseCategory, dt.ExpenseCategoryDetails }
                                    where ec.Id != dt.Id
                                    select new
                                    {
                                        ShipmentType = dt.ShipmentType,
                                        dt.Category
                                        ,
                                        dt.ExpenseCategory
                                        ,
                                        dt.ExpenseCategoryDetails

                                    }).ToList();


            var duplicatelst = datas
                        .GroupBy(x => new { x.ShipmentType, x.Category, x.ExpenseCategory, x.ExpenseCategoryDetails })
                        .Select(y => new
                        {
                            ShipmentType = y.Key.ShipmentType,
                            Category = y.Key.Category,
                            ExpenseCategory = y.Key.ExpenseCategory,
                            ExpenseCategoryDetails = y.Key.ExpenseCategoryDetails,
                            TotalCnt = y.ToList()
                        }).ToList();


            if (lexistingRecords.Count > 0)
            {
                ModelState.AddModelError(lexistingRecords.Select(x => string.Join("-", x.ShipmentType, x.Category, x.ExpenseCategory, x.ExpenseCategoryDetails)).ToList());
            }

            else if (duplicatelst.Any(x => x.TotalCnt.Count > 1))
            {
                ModelState.AddModelError(duplicatelst.Where(x => x.TotalCnt.Count > 1).Select(x => string.Join("-", x.ShipmentType, x.Category, x.ExpenseCategory, x.ExpenseCategoryDetails)).ToList());
            }
            else
            {
                foreach (var item in datas)
                {
                    if (!await item.SaveAsync())
                        ModelState.AddModelError(item.ErrorMessage);
                }
            }
            return datas.ToDataSourceResult(request, ModelState);
        }
        [ValidateAction(Forms.Procurement.ControlPanel, Rights.Delete)]
        public async Task<ReturnMessage> DeleteShipmentSetting([FromForm] ShipmentExpenseCategory data)
        {

            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Shipment setting record deleted successfully.");
        }
        public async Task<ReturnMessage> RevertItem([FromForm] ShipmentExpenseCategory data)
        {
            if (!await data.RejectAsync())
                return UndoError(data.ErrorMessage);

            return Message("Reverted Successfully.");
        }
        #endregion

        #region Item Category Setting
        [ValidateAction(Forms.Procurement.ItemCategorySetting, Rights.View)]
        public async Task<DataSourceResult> ReadItemCategorySettings([DataSourceRequest] DataSourceRequest request)
        {
            var result = (from SPC in _Webcontext.ItemCategorySettings
                          join Ic in _Webcontext.ItemCategories on SPC.ItemCategoryId equals Ic.Id
                          select new ItemCategorySetting
                          {
                              ID = SPC.ID,
                              ItemCategory = Ic.Category,
                              OrderCategory = SPC.OrderCategory,
                              ABCRanking = SPC.ABCRanking,
                              AnnualScheduledOrder = SPC.AnnualScheduledOrder,
                              OrderFrequency = SPC.OrderFrequency,
                              StockHoldingMonths = SPC.StockHoldingMonths,
                              InitialOrderQty = SPC.InitialOrderQty,
                              GST = SPC.GST,
                              Colour = SPC.Colour,
                              SHRankA = SPC.SHRankA,
                              SHRankB = SPC.SHRankB,
                              SHRankC = SPC.SHRankC,
                              SHRankD = SPC.SHRankD,
                              OrderMinimum = SPC.OrderMinimum,
                              ItemCategoryId = SPC.ItemCategoryId
                          });
            return await result.ToDataSourceResultAsync(request);
        }
        public async Task<List<DropDownModel>> ReadItemCategory()
        {
            return await _Webcontext.ItemCategories.Select(x => new DropDownModel { Id = x.Id, Text = x.Category }).ToListAsync();
        }

        [ValidateAction(Forms.Procurement.ItemCategorySetting, Rights.Modify)]
        public async Task<DataSourceResult> UpdateItemCategory([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "Models")] IEnumerable<ItemCategorySetting> datas)
        {
            var existingList = await (from ec in _Webcontext.ItemCategorySettings
                                      join ic in _Webcontext.ItemCategories on ec.ItemCategoryId equals ic.Id
                                      select new
                                      {
                                          ec.OrderCategory,
                                          ec.ItemCategoryId,
                                          ic.Category,
                                          ec.ID
                                      }
                                     ).ToListAsync();

            var lexistingRecords = (from ec in existingList
                                    join dt in datas on new { ec.ItemCategoryId, ec.OrderCategory } equals new { dt.ItemCategoryId, dt.OrderCategory }
                                    where ec.ID != dt.ID
                                    select new
                                    {
                                        dt.OrderCategory,
                                        dt.ItemCategoryId,
                                        ec.Category
                                    }).ToList();


            var duplicatelst = datas
                        .GroupBy(x => new { x.OrderCategory, x.ItemCategory })
                        .Select(y => new
                        {
                            OrderCategory = y.Key.OrderCategory,
                            Category = y.Key.ItemCategory,
                            TotalCnt = y.ToList()
                        }).ToList();


            if (lexistingRecords.Count > 0)
            {
                ModelState.AddModelError(lexistingRecords.Select(x => "This record exists. (" + string.Join("-", x.Category, x.OrderCategory) + ")").ToList());
            }

            else if (duplicatelst.Any(x => x.TotalCnt.Count > 1))
            {
                ModelState.AddModelError(duplicatelst.Where(x => x.TotalCnt.Count > 1).Select(x => "Record is duplicated. (" + string.Join("-", x.Category, x.OrderCategory) + ")").ToList());
            }
            else
            {
                foreach (var item in datas)
                {
                    if (!await item.SaveAsync())
                        ModelState.AddModelError(item.ErrorMessage);
                }
            }
            return datas.ToDataSourceResult(request, ModelState);
        }
        [ValidateAction(Forms.Procurement.ItemCategorySetting, Rights.Delete)]
        public async Task<ReturnMessage> DeleteItemCategory([FromForm] ItemCategorySetting data)
        {

            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Deleted successfully.");
        }
        #endregion 
    }

}

