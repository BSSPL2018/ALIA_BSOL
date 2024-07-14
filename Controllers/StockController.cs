using BSOL.Core;
using BSOL.Core.Entities;
using BSOL.Core.Models.Common;
using BSOL.Core.Models.Procurement;
using BSOL.Helpers;
using BSOL.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BSOL.Controllers
{
    public class StockController : BaseController
    {
        private readonly ICommonHelper _commonHelper;
        private readonly NotificationService _notificationService;
        public StockController(BSOLContext context, BSOLWebContext Webcontext, AppUser appUser, ICommonHelper commonHelper, NotificationService notificationService) : base(context, Webcontext, appUser)
        {
            _commonHelper = commonHelper;
            _notificationService = notificationService;
        }

        #region Non Serial Items
        [ValidateAction(Forms.Procurement.ReceiveItems, Rights.Modify)]
        public async Task<ReturnMessage> ReceiveItems([FromForm] IEnumerable<ItemStatus> datas)
        {
            var tableSchema = new List<SqlMetaData>() {
                new SqlMetaData("ItemCostingId", SqlDbType.BigInt),
                new SqlMetaData("ReceivedQty", SqlDbType.Decimal,10,2),
            }.ToArray();
            var sqlDataRecords = new List<SqlDataRecord>();
            datas.ToList().ForEach(x =>
            {
                SqlDataRecord sqlDataRecord = new SqlDataRecord(tableSchema);
                sqlDataRecord.SetInt64(0, x.ItemCostingId);
                sqlDataRecord.SetDecimal(1, x.ReceivedQty);
                sqlDataRecords.Add(sqlDataRecord);
            });

            List<SqlParameter> sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@Option", "VAL_RECEIVE"),
                new SqlParameter("@ReceivedBy", _appUser.CommonEmpNo),
                new SqlParameter
                {
                    ParameterName = "@ReceivedItems",
                    Value = sqlDataRecords,
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.ReceivedItemsType"
                }
            };
            var errors = await _Webcontext.ValidateSqlAsync("spBSOL_ReceiveNonSerialedItems", sqlParams);
            if (errors.Any())
                return Error(errors);

            sqlParams[0].Value = "RECEIVE_ITEMS";
            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("spBSOL_ReceiveNonSerialedItems", sqlParams);
            if (retVal <= 0)
                return DBError();

            return Message("Items received.");
        }
        #endregion 
        #region Receive Serial Items
        [ValidateAction(Forms.Procurement.ReceiveItems, Rights.View)]
        public async Task<DataSourceResult> ReadPerishableItems([DataSourceRequest] DataSourceRequest request, [FromQuery] int statusFilter, [FromForm] DateTime? fromDate, [FromForm] DateTime? toDate)
        {
            if (statusFilter == 0)/* Un Cleared Shipments */
            {
                //request.Sorts.Default(nameof(ItemStatus.ClearedDate), Kendo.Mvc.ListSortDirection.Descending);

                var data = (from sh in _Webcontext.Shipments
                            join spod in _Webcontext.ShipmentPurchaseOrderDetails on sh.Id equals spod.ShipmentId
                            join pod in _Webcontext.PurchaseOrderDetails on spod.PurchaseOrderDetailId equals pod.Id
                            join po in _Webcontext.PurchaseOrders on pod.PurchaseOrderId equals po.Id
                            join im in _Webcontext.Items on pod.ItemId equals im.Id
                            join ic in _Webcontext.ItemCategories on im.ItemCategoryId equals ic.Id
                            where sh.CompanyId == _appUser.CompanyId && sh.Active
                            && po.Mode == "Shipment" && !sh.ClearedDate.HasValue
                            select new ItemStatus
                            {
                                RefNo = sh.RefNoFormatted,
                                Category = ic.Category,
                                SKU = im.SKU,
                                UPC = im.UPC,
                                Description = im.Description,
                                ConfirmedQty = spod.ConfirmedQty,
                                ReceivedQty = 0,
                                ClearedDate = sh.ClearedDate,
                                ItemCode = im.ItemCode,
                                SKUFormatted = im.SKUFormatted,
                                Unit = im.Unit,
                                Size = im.Size
                            });
                return await data.ToDataSourceResultAsync(request);
            }
            else
            {
                //if (statusFilter == 2)/* Received */
                //    request.Sorts.Default(nameof(ItemStatus.ReceivedDate), Kendo.Mvc.ListSortDirection.Descending);
                //else
                //    request.Sorts.Default(nameof(ItemStatus.EntryDate), Kendo.Mvc.ListSortDirection.Descending);

                var data = (from ic in _Webcontext.ItemCostings
                            join im in _Webcontext.Items on ic.ItemId equals im.Id
                            join ct in _Webcontext.ItemCategories on im.ItemCategoryId equals ct.Id
                            join lsh in _Webcontext.Shipments on ic.ShipmentId equals lsh.Id into grpSH
                            from sh in grpSH.DefaultIfEmpty()
                            join lpo in _Webcontext.PurchaseOrders on ic.PurchaseOrderId equals lpo.Id into grpPO
                            from po in grpPO.DefaultIfEmpty()
                            where ic.VerifiedOn.HasValue
                            && ((statusFilter == 1 && !ic.ReceivedDate.HasValue)
                            || (statusFilter == 2 && ic.ReceivedDate.HasValue && ic.ReceivedDate.Value.Date >= fromDate && ic.ReceivedDate.Value.Date <= toDate)
                            )
                            select new ItemStatus
                            {
                                ItemCostingId = ic.ID,
                                ItemId = ic.ItemId,
                                RefNo = sh.RefNoFormatted ?? po.RefNoFormatted,
                                Category = ct.Category,
                                SKU = im.SKU,
                                Description = im.Description,
                                ConfirmedQty = ic.ConfirmedQty,
                                ReceivedQty = ic.ReceivedDate != null ? ic.ReceivedQty : ic.ConfirmedQty,
                                ReceivedBy = ic.ReceivedBy,
                                ReceivedDate = (DateTime?)ic.ReceivedDate,
                                ItemCode = im.ItemCode,
                                SKUFormatted = im.SKUFormatted,
                                Unit = ic.Unit,
                                Size = im.Size,
                                ClearedDate = sh.ClearedDate,
                                EntryDate = (long?)sh.Id > 0 ? sh.CreatedOn : po.CreatedOn,
                                IsNonSerialed = (!im.IsPerishable && !im.IsSerialized)
                            });
                return await data.ToDataSourceResultAsync(request);
            }
        }

        public async Task<ReturnMessage> SavePerishableItems([FromForm] long itemCostingId, [FromForm] long itemId, [FromForm] string refNo, [FromForm] List<SerialedItem> perishableItems)
        {
            var tableSchema = new List<SqlMetaData>() {
                new SqlMetaData("Id", SqlDbType.BigInt),
                new SqlMetaData("SerialNo", SqlDbType.VarChar, 100),
                new SqlMetaData("Qty", SqlDbType.Decimal,18,2),
                new SqlMetaData("ExpiryDate", SqlDbType.Date),
            }.ToArray();
            var sqlDataRecords = new List<SqlDataRecord>();
            perishableItems.ForEach(x =>
            {
                SqlDataRecord sqlDataRecord = new SqlDataRecord(tableSchema);
                sqlDataRecord.SetInt64(0, x.Id);
                sqlDataRecord.SetString(1, x.SerialNo ?? "");
                sqlDataRecord.SetDecimal(2, x.Qty);
                sqlDataRecord.SetDateTime(3, x.ExpiryDate.Value);
                sqlDataRecords.Add(sqlDataRecord);
            });

            List<SqlParameter> sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@Option", "VAL_SAVE"),
                new SqlParameter("@CompanyId", _appUser.CompanyId),
                new SqlParameter("@ItemCostingId", itemCostingId),
                new SqlParameter("@CreatedBy", _appUser.CommonEmpNo),
                new SqlParameter("@ItemId", itemId),
                new SqlParameter
                {
                    ParameterName = "@PerishableItems",
                    Value = sqlDataRecords,
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.ShipmentPerishableItemType"
                }
            };

            var errors = await _Webcontext.ValidateSqlAsync("spBSOL_ReceivePerishableItems", sqlParams);
            if (errors.Any())
                return SaveError(errors);

            sqlParams[0].Value = "INSERT";
            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("spBSOL_ReceivePerishableItems", sqlParams);
            if (retVal <= 0)
                return DBError();

            await EventLogAsync("PERISHABLE ITEMS", ActionType.Edit, "SerialedItems", itemCostingId, refNo, perishableItems.Count);

            return Message("Perishable Items saved.");
        }

        [ValidateAction(Forms.Procurement.ReceiveItems, Rights.Delete)]
        public async Task<ReturnMessage> DeletePerishableItems([FromForm] long itemCostingId, [FromForm] string refNo)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_ReceivePerishableItems", new { Option = "VAL_DELETE", itemCostingId });
            if (errors.Any())
                return DeleteError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_ReceivePerishableItems", new { Option = "DELETE", itemCostingId });
            if (retVal <= 0)
                return DBError();

            await EventLogAsync("EXPIRABLE ITEMS", ActionType.Delete, "SerialedItems", itemCostingId, refNo);

            return Message("Expirable Items Deleted.");
        }

        [ValidateAction(Forms.Procurement.ReceiveItems, Rights.Modify)]
        public async Task<ReturnMessage> ReceivePerishableItems([FromForm] long itemCostingId, [FromForm] string refNo)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_ReceivePerishableItems", new { Option = "VAL_RECEIVE", itemCostingId });
            if (errors.Any())
                return Error(errors, "Cannot Receive");

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_ReceivePerishableItems", new { Option = "RECEIVE_ITEMS", itemCostingId, ReceivedBy = _appUser.CommonEmpNo });
            if (retVal <= 0)
                return DBError();

            await EventLogAsync("EXPIRABLE ITEMS", ActionType.Edit, "SerialedItems", itemCostingId, "Received", refNo);

            return Message("Expirable Items Received.");
        }

        [ValidateAction(Forms.Procurement.ReceiveItems, Rights.Modify)]
        public async Task<ReturnMessage> UnReceivePerishableItems([FromForm] long itemCostingId, [FromForm] string refNo)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_ReceivePerishableItems", new { Option = "VAL_UN_RECEIVE", itemCostingId });
            if (errors.Any())
                return Error(errors, "Cannot Un-Receive");

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_ReceivePerishableItems", new { Option = "UN_RECEIVE_ITEMS", itemCostingId });
            if (retVal <= 0)
                return DBError();

            await EventLogAsync("EXPIRABLE ITEMS", ActionType.Edit, "SerialedItems", itemCostingId, "Un-Received", refNo);

            return Message("Expirable Items Un-Received.");
        }

        public async Task<DataSourceResult> ReadSIPerishableItems([DataSourceRequest] DataSourceRequest request, [FromForm] long itemCostingId)
        {
            var result = (from sn in _Webcontext.SerialedItems
                          join im in _Webcontext.Items on sn.ItemId equals im.Id
                          join ic in _Webcontext.ItemCostings on sn.ItemCostingId equals ic.ID
                          where sn.ItemCostingId == itemCostingId
                          select new SerialedItem
                          {
                              Id = sn.Id,
                              ItemCostingId = sn.ItemCostingId,
                              ItemId = sn.ItemId,
                              SerialNo = sn.SerialNo,
                              Qty = sn.ActualQty,
                              Stock = sn.Stock,
                              ExpiryDate = sn.ExpiryDate,
                              ReceivedBy = sn.ReceivedBy,
                              ReceivedDate = sn.ReceivedDate,
                              CreatedBy = sn.CreatedBy,
                              CreatedOn = sn.CreatedOn,
                              SKU = im.SKU,
                              Description = im.Description,
                              Size = im.Size,
                              ItemCode = im.ItemCode,
                              Unit = ic.Unit
                          });
            return await result.ToDataSourceResultAsync(request);
        }
        public async Task<ReturnMessage> DeleteSerialedItem([FromForm] long id, [FromForm] string refNo = "")
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_ReceivePerishableItems", new { Option = "VAL_DELETE_SINGLE", id });
            if (errors.Any())
                return DeleteError(errors);

            var existing = await _Webcontext.SerialedItems.Where(x => x.Id == id).FirstOrDefaultAsync();

            _Webcontext.Remove(existing);
            await _Webcontext.SaveChangesAsync();

            await EventLogAsync("Serialed Item", ActionType.Delete, "SerialedItems", id, refNo);
            return Message("Entry deleted successfully.");
        }

        #endregion
    }
}
