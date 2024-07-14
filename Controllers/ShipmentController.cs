using BSOL.Core;
using BSOL.Core.Entities;
using BSOL.Core.Helpers;
using BSOL.Core.Models.Common;
using BSOL.Helpers;
using BSOL.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using BSOL.Core.Models.Shipment;

namespace BSOL.Controllers
{
    public class ShipmentController : BaseController
    {
        private readonly ICommonHelper _commonHelper;
        private readonly NotificationService _notificationService;
        public ShipmentController(BSOLContext context, BSOLWebContext Webcontext, AppUser appUser, ICommonHelper commonHelper, NotificationService notificationService) : base(context, Webcontext, appUser)
        {
            _commonHelper = commonHelper;
            _notificationService = notificationService;
        }
        #region Shipments
        [ValidateAction(Forms.Procurement.Shipments, Rights.View)]
        public async Task<DataSourceResult> ReadShipments([DataSourceRequest] DataSourceRequest request, [FromQuery] int statusFilter)
        {
            var result = _Webcontext.Shipments.Where(x => x.CompanyId == _appUser.CompanyId && x.Active && (statusFilter == 0 ? !x.ClearedDate.HasValue : x.ClearedDate.HasValue));
            return await result.ToDataSourceResultAsync(request);
        }

        [ValidateAction(Forms.Procurement.Shipments, Rights.Modify)]
        public async Task<ReturnMessage> SaveShipment([FromForm] Shipment shipment, [FromForm] List<ShipmentPurchaseOrderDetail> purchaseOrderDetails)
        {
            var incoTerm = (from pl in purchaseOrderDetails
                            join pod in _Webcontext.PurchaseOrderDetails on pl.PurchaseOrderDetailId equals pod.Id
                            join po in _Webcontext.PurchaseOrders on pod.PurchaseOrderId equals po.Id
                            select new
                            {
                                po.IncoTerm
                            }).Distinct();

            if (incoTerm.Where(x => x.IncoTerm != shipment.TradeTerm).Select(x => x).Distinct().Count() > 0)
            {
                return Error("Shipment Trade Term and Purchase Order Incoterm should be same.");
            }

            shipment.CompanyId = _appUser.CompanyId;
            if (!await shipment.SaveAsync())
                return SaveError(shipment.ErrorMessage);

            var tableSchema = new List<SqlMetaData>() {
                new SqlMetaData("PurchaseOrderDetailId", SqlDbType.BigInt),
                new SqlMetaData("ConfirmedQty", SqlDbType.Decimal,18,2),
            }.ToArray();
            var sqlDataRecords = new List<SqlDataRecord>();
            purchaseOrderDetails.ForEach(x =>
            {
                SqlDataRecord sqlDataRecord = new SqlDataRecord(tableSchema);
                sqlDataRecord.SetInt64(0, x.PurchaseOrderDetailId);
                sqlDataRecord.SetDecimal(1, x.ConfirmedQty);
                sqlDataRecords.Add(sqlDataRecord);
            });

            List<SqlParameter> sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@Option", "UPDATE"),
                new SqlParameter("@ShipmentId", shipment.Id),
                new SqlParameter
                {
                    ParameterName = "@ShipmentPurchaseOrderDetails",
                    Value = sqlDataRecords,
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.ShipmentPurchaseOrderDetailType"
                }
            };

            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("spBSOL_Shipment", sqlParams);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("SHIPMENT", ActionType.Add.ToString(), "Shipment", shipment.Id, shipment.RefNoFormatted);
            return Message("Shipment saved.", shipment.Id);
        }

        [ValidateAction(Forms.Procurement.Shipments, Rights.Modify)]
        public async Task<ReturnMessage> ProcessShipment([FromForm] Shipment data)
        {
            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_Shipment", new { Option = "CLEAR_SHIPMENT", ShipmentId = data.Id, data.ClearedDate, _appUser.ShopId, CreatedBy = _appUser.CommonEmpNo });
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("PROCESS SHIPMENT", ActionType.Edit.ToString(), "Shipment", data.Id, data.RefNoFormatted);
            return Message("Shipment details saved.");
        }

        [ValidateAction(Forms.Procurement.Shipments, Rights.Modify)]
        public async Task<ReturnMessage> UndoShipment([FromForm] Shipment data)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_Shipment", new { Option = "VAL_UNDO", ShipmentId = data.Id });
            if (errors.Any())
                return Error(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_Shipment", new { Option = "UNDO_SHIPMENT", ShipmentId = data.Id });
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("UNDO SHIPMENT", ActionType.Edit.ToString(), "Shipment", data.Id, data.RefNoFormatted);
            return Message("Shipment details saved.");
        }

        [ValidateAction(Forms.Procurement.Shipments, Rights.Delete)]
        public async Task<ReturnMessage> DeleteShipment([FromForm] Shipment data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_Shipment", new { Option = "DELETE", ShipmentId = data.Id });
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await new Document() { DocumentRoot = _commonHelper.GetDocumentRoot() }.DeleteByReference(DocumentReference.Shipments, data.Id.ToString());

            await EventLog("SHIPMENT", ActionType.Delete.ToString(), "Shipment", data.Id, data.RefNoFormatted);
            return Message("Shipment deleted.");
        }

        public async Task<List<string>> ReadOrigins()
        {
            return await _Webcontext.Ports.Where(x => x.CompanyId == _appUser.CompanyId).Select(x => x.CountryName).Distinct().ToListAsync();
        }

        public async Task<List<string>> ReadOriginPorts([FromForm] string origin)
        {
            return await _Webcontext.Ports.Where(x => x.CountryName == origin).Select(x => x.PortName).ToListAsync();
        }

        public async Task<DataSourceResult> ReadPurchaseOrders([DataSourceRequest] DataSourceRequest request)
        {
            var result = await (from po in _Webcontext.PurchaseOrders
                                join s in _Webcontext.Suppliers on po.SupplierId equals s.Id
                                where s.CompanyId == _appUser.CompanyId && po.Active && po.VerifiedOn.HasValue
                                && po.Mode == "Shipment" && s.Active && !po.ClosedOn.HasValue
                                && (from pod in _Webcontext.PurchaseOrderDetails
                                    join im in _Webcontext.Items on pod.ItemId equals im.Id
                                    where pod.PurchaseOrderId == po.Id && (pod.RequestedQty - pod.ConfirmedQty) > 0 && im.IsInventory
                                    select pod.Id).Any()
                                select new
                                {
                                    po.Id,
                                    po.RefNoFormatted,
                                    s.SupplierName,
                                    po.TotalAmount,
                                    po.PurchaseOrderDate,
                                    po.CreatedBy,
                                    po.CreatedOn
                                }).ToListAsync();
            return result.ToDataSourceResult(request);
        }

        public async Task<DataSourceResult> ReadShipmentPurchaseOrders([DataSourceRequest] DataSourceRequest request, [ModelBinder(typeof(IdentityDecryptor))] long shipmentId)
        {
            var result = await (from spod in _Webcontext.ShipmentPurchaseOrderDetails
                                join pod in _Webcontext.PurchaseOrderDetails on spod.PurchaseOrderDetailId equals pod.Id
                                join po in _Webcontext.PurchaseOrders on pod.PurchaseOrderId equals po.Id
                                join im in _Webcontext.Items on pod.ItemId equals im.Id
                                where spod.ShipmentId == shipmentId
                                select new ShipmentPurchaseOrderDetail
                                {
                                    ShipmentId = spod.ShipmentId,
                                    PurchaseOrderDetailId = spod.PurchaseOrderDetailId,
                                    ConfirmedQty = spod.ConfirmedQty,
                                    /* Additional Properties */
                                    PurchaseOrderId = pod.PurchaseOrderId,
                                    ItemId = pod.ItemId,
                                    Description = pod.Description,
                                    RequestedQty = (pod.RequestedQty - pod.ConfirmedQty) + spod.ConfirmedQty,
                                    RefNoFormatted = po.RefNoFormatted,
                                    SKU = im.SKU,
                                    UPC = im.UPC
                                }).ToListAsync();
            return result.ToDataSourceResult(request);
        }

        public async Task<List<ShipmentPurchaseOrderDetail>> GetPurchaseOrderDetails([FromForm] long purcharseOrderId)
        {
            return await (from pod in _Webcontext.PurchaseOrderDetails
                          join po in _Webcontext.PurchaseOrders on pod.PurchaseOrderId equals po.Id
                          join im in _Webcontext.Items on pod.ItemId equals im.Id
                          where pod.PurchaseOrderId == purcharseOrderId && po.Active && po.VerifiedOn.HasValue && (pod.RequestedQty - pod.ConfirmedQty) > 0 && im.IsInventory
                          select new ShipmentPurchaseOrderDetail
                          {
                              PurchaseOrderDetailId = pod.Id,
                              /* Additional Properties */
                              PurchaseOrderId = pod.PurchaseOrderId,
                              ItemId = pod.ItemId,
                              Description = pod.Description,
                              RequestedQty = pod.RequestedQty - pod.ConfirmedQty,
                              ConfirmedQty = pod.RequestedQty - pod.ConfirmedQty,
                              RefNoFormatted = po.RefNoFormatted,
                              SKU = im.SKU
                          }).ToListAsync();
        }

        [ValidateAction(Forms.Procurement.Shipments, Rights.Modify)]
        public async Task<Shipment> GetShipment([ModelBinder(typeof(IdentityDecryptor))] long id)
        {
            return await _Webcontext.Shipments.FindAsync(id);
        }
        #endregion
        #region Expenses
        [ValidateAction(Forms.Procurement.Shipments, Rights.View)]
        public async Task<DataSourceResult> ReadShipmentExpenses([DataSourceRequest] DataSourceRequest request, [ModelBinder(typeof(IdentityDecryptor))] long shipmentId)
        {
            var result = (from sec in _Webcontext.ShipmentExpenseCategories
                          join sh in _Webcontext.Shipments on sec.ShipmentType equals sh.ShipmentMode
                          join lse in _Webcontext.ShipmentExpenses on new { ExpenseCategoriesId = sec.Id, ShipmentId = sh.Id } equals new { lse.ExpenseCategoriesId, lse.ShipmentId } into grpse
                          from se in grpse.DefaultIfEmpty()
                          join lgs in _Webcontext.GSTSettings on se.GSTSettingId equals lgs.Id into grpgs
                          from gs in grpgs.DefaultIfEmpty()
                          where (se.Id > 0 || sec.Active) && sh.Id == shipmentId
                          select new ShipmentExpense
                          {
                              Id = (long)se.Id,
                              ExpenseName = sec.ExpenseCategory,
                              Description = sec.ExpenseCategoryDetails,
                              Currency = se.Currency,
                              Amount = se.Amount,
                              ShipmentId = sh.Id,
                              ExpenseCategoriesId = sec.Id,
                              ExpenseDate = se.ExpenseDate,
                              EntryBy = se.EntryBy,
                              EntryDate = se.EntryDate,
                              GST = gs.Name,
                              GSTSettingId = se.GSTSettingId,
                              GSTAmount = se.GSTAmount,
                              GSTPercent = se.GSTPercent,
                              TotalAmount = se.TotalAmount
                          }).OrderByDescending(x => x.Id).ToList();
            return await result.ToDataSourceResultAsync(request);
        }

        public async Task<List<DropDownModel>> ReadExpenseNames()
        {
            return await _Webcontext.ShipmentExpenses.Select(x => new DropDownModel { Text = x.ExpenseName }).Distinct().ToListAsync();
        }

        [ValidateAction(Forms.Procurement.Shipments, Rights.Modify)]
        public async Task<ReturnMessage> UpdateShipmentExpenses([FromForm] List<ShipmentExpense> datas)
        {
            foreach (var shipmentExpense in datas)
            {
                if (!await shipmentExpense.SaveAsync())
                    return Error(Constants.DBErrorMessage);
            }
            return Message("Saved Successfully");
        }

        [ValidateAction(Forms.Procurement.Shipments, Rights.Delete)]
        public async Task<ReturnMessage> DeleteShipmentExpenses([FromForm] ShipmentExpense data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);
            return Message("Shipment expenses deleted successfully.");
        }

        #endregion
        #region Serialed Items
        public async Task<DataSourceResult> ReadSerialedShipments([DataSourceRequest] DataSourceRequest request, [FromForm] int statusFilter)
        {
            var result = await _Webcontext.ExecuteSpAsync<ShipmentPOSerialModel>("spBSOL_ShipmentSerialedItems", new { Option = "SELECT_SHIPMENT_PO", _appUser.CompanyId, statusFilter });
            return await result.ToDataSourceResultAsync(request);
        }

        public async Task<DataSourceResult> ReadSerialedItems([DataSourceRequest] DataSourceRequest request, [FromForm] long ItemCostingId)
        {
            request.Sorts.SortInterceptor("SKU");

            var result = await _Webcontext.ExecuteSpAsync<SerialedItemModel>("spBSOL_ShipmentSerialedItems", new { Option = "SELECT", ItemCostingId });
            return await result.ToDataSourceResultAsync(request);
        }

        [ValidateAction(Forms.Procurement.Shipments, Rights.Modify)]
        public async Task<ReturnMessage> SaveSerialedItems([FromForm] long ItemCostingId, [FromForm] string refNo, [FromForm] List<SerialedItemModel> serialedItems)
        {

            var tableSchema = new List<SqlMetaData>() {
                new SqlMetaData("Id", SqlDbType.BigInt),
                new SqlMetaData("ItemId", SqlDbType.BigInt),
                new SqlMetaData("SerialNo", SqlDbType.VarChar, 100)
            }.ToArray();
            var sqlDataRecords = new List<SqlDataRecord>();
            serialedItems.ForEach(x =>
            {
                SqlDataRecord sqlDataRecord = new SqlDataRecord(tableSchema);
                sqlDataRecord.SetInt64(0, x.Id ?? 0);
                sqlDataRecord.SetInt64(1, x.ItemId);
                sqlDataRecord.SetString(2, x.SerialNo ?? "");
                sqlDataRecords.Add(sqlDataRecord);
            });

            List<SqlParameter> sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@Option", "VAL_SAVE"),
                new SqlParameter("@CompanyId", _appUser.CompanyId),
                new SqlParameter("@ItemCostingId", ItemCostingId),
                new SqlParameter("@CreatedBy", _appUser.CommonEmpNo),
                new SqlParameter
                {
                    ParameterName = "@ShipmentSerialedItems",
                    Value = sqlDataRecords,
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.ShipmentSerialedItemType"
                }
            };

            var errors = await _Webcontext.ValidateSqlAsync("spBSOL_ShipmentSerialedItems", sqlParams);
            if (errors.Any())
                return SaveError(errors);

            sqlParams[0].Value = "INSERT";
            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("spBSOL_ShipmentSerialedItems", sqlParams);
            if (retVal <= 0)
                return DBError();

            await EventLogAsync("SERIALED ITEMS", ActionType.Edit, "SerialedItems", ItemCostingId, refNo, serialedItems.Count);

            return Message("Serialed Items saved.");
        }

        [ValidateAction(Forms.Procurement.Shipments, Rights.Delete)]
        public async Task<ReturnMessage> DeleteSerialedItems([FromForm] long ItemCostingId, [FromForm] string refNo)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_ShipmentSerialedItems", new { Option = "VAL_DELETE", ItemCostingId });
            if (errors.Any())
                return DeleteError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_ShipmentSerialedItems", new { Option = "DELETE", ItemCostingId });
            if (retVal <= 0)
                return DBError();

            await EventLogAsync("SERIALED ITEMS", ActionType.Delete, "SerialedItems", ItemCostingId, refNo);

            return Message("Serialed Items deleted.");
        }

        #endregion
    }
}
