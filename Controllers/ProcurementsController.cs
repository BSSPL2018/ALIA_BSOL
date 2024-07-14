using Microsoft.AspNetCore.Mvc;
using BSOL.Core;
using BSOL.Core.Models.Common;
using BSOL.Helpers;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using BSOL.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using BSOL.Core.Models.Logitics;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Net.Mime;
using BSOL.Services;
using Microsoft.Data.SqlClient.Server;
using BSOL.Core.Helpers;
using BSOL.Core.Models.Procurement;
using BSOL.Core.Models.Retail;
using System.Data.SqlTypes;
using DetailedValidationModel = BSOL.Core.Models.General.DetailedValidationModel;

using Microsoft.Data.SqlClient;
using SQLitePCL;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Vml.Office;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Reflection.Emit;
using Document = BSOL.Core.Entities.Document;
using BSOL.Core.Models.Accounts;

namespace BSOL.Controllers
{
    [Authorize]
    public class ProcurementsController : BaseController
    {
        private readonly ICommonHelper _commonHelper;
        private readonly NotificationService _notificationService;
        public ProcurementsController(BSOLContext context, BSOLWebContext Webcontext, AppUser appUser, ICommonHelper commonHelper, NotificationService notificationService) : base(context, Webcontext, appUser)
        {
            _commonHelper = commonHelper;
            _notificationService = notificationService;
        }
        #region Selling Units
        public async Task<List<DropDownModel>> ReadUnitOfMeasureData()
        {
            return await _Webcontext.UnitOfMeasures.Select(x => new DropDownModel { Id = x.ID, Text = x.UOM }).ToListAsync();
        }
        [ValidateAction(Forms.Procurement.Items, Rights.View)]
        public async Task<DataSourceResult> ReadSellingItemUnits([DataSourceRequest] DataSourceRequest request, [FromForm] long ItemId = 0)
        {
            var result = (from i in _Webcontext.Items
                          join si in _Webcontext.SellingItemUnits on i.Id equals si.ItemId
                          join um in _Webcontext.UnitOfMeasures on si.SellingUnitId equals um.ID
                          where i.Id == ItemId
                          select new SellingItemUnit
                          {
                              ItemId = si.ItemId,
                              Id = si.Id,
                              SellingUnitId = si.SellingUnitId,
                              SellingUnitRate = si.SellingUnitRate,
                              UPC = si.UPC,
                              Conversion = si.Conversion,
                              SellingUnit = um.UOM
                          });
            return await result.ToDataSourceResultAsync(request);
        }


        [ValidateAction(Forms.Procurement.Items, Rights.Modify)]
        public async Task<DataSourceResult> UpdateSellingItemUnits([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "models")] IEnumerable<SellingItemUnit> datas)
        {
            foreach (var item in datas)
            {
                if (!await item.SaveAsync())
                    ModelState.AddModelError(item.ErrorMessage);
            }
            return datas.ToDataSourceResult(request, ModelState);
        }

        [ValidateAction(Forms.Procurement.Items, Rights.Delete)]
        public async Task<ReturnMessage> DeleteItemSellingUnits([FromForm] SellingItemUnit data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Selling Unit deleted.");
        }
        #endregion
        #region Items
        [ValidateAction(Forms.Procurement.Items, Rights.View)]
        public async Task<DataSourceResult> GetItems([DataSourceRequest] DataSourceRequest request, [FromForm] int statusFilter = 0)
        {
            var result = (from i in _Webcontext.Items
                          join ic in _Webcontext.ItemCategories on i.ItemCategoryId equals ic.Id
                          join s in _Webcontext.SalesCategories on i.SalesCategoryId equals s.Id into grpSC
                          from sc in grpSC.DefaultIfEmpty()
                          join sp in _Webcontext.Suppliers on i.SupplierId equals sp.Id into grpSP
                          from lsp in grpSP.DefaultIfEmpty()
                          join h in _Webcontext.HSNSettings on i.HSNId equals h.Id into hsnSetting
                          from hsn in hsnSetting.DefaultIfEmpty()
                          where statusFilter == 0 ? i.Active && !i.VerifiedOn.HasValue    /* Pending */
                                : statusFilter == 1 ? i.Active && i.VerifiedOn.HasValue  /* Verified */
                                : !i.Active                                               /* InActive */
                          orderby i.SKU
                          select new Item
                          {
                              Id = i.Id,
                              ItemCategoryId = i.ItemCategoryId,
                              SalesCategoryId = i.SalesCategoryId,
                              ItemType = i.ItemType,
                              SKU = i.SKU,
                              UPC = i.UPC,
                              SKUFormatted = i.SKUFormatted,
                              Description = i.Description,
                              Unit = i.Unit,
                              Size = i.Size,
                              IsSalable = i.IsSalable,
                              IsInventory = i.IsInventory,
                              IsPerishable = i.IsPerishable,
                              IsSerialized = i.IsSerialized,
                              HSNId = i.HSNId,
                              SupplierName = lsp.SupplierName,
                              SupplierId = lsp.Id,
                              PurchasedRate = i.PurchasedRate,
                              SellingRate = i.SellingRate,
                              Stock = i.Stock,
                              OpeningStock = i.OpeningStock,
                              OpeningCostPrice = i.OpeningCostPrice,
                              OpeningStockDate = i.OpeningStockDate,
                              LowStockAlertQty = i.LowStockAlertQty,
                              GSTApplicable = i.GSTApplicable,
                              ImagePath = i.ImagePath,
                              Notes = i.Notes,
                              Active = i.Active,
                              VerifiedBy = i.VerifiedBy,
                              VerifiedOn = i.VerifiedOn,
                              CreatedBy = i.CreatedBy,
                              CreatedOn = i.CreatedOn,
                              VendorName = i.VendorName,
                              OriginName = i.OriginName,
                              Brand = i.Brand,
                              /* Additional */
                              Category = ic.Category,
                              SalesCategory = sc.Category,
                              HSNCode = hsn.HSNCode,
                              ItemCode = i.ItemCode,
                              SellingUnit = i.SellingUnit,
                              Surcharge = i.Surcharge,
                              ModelCode = i.ModelCode,
                              Color = i.Color,
                              Height = i.Height,
                              Length = i.Length,
                              Width = i.Width,
                              CBM = i.CBM,
                              Import = i.Import,
                              GenericName = i.GenericName
                          });

            return await result.ToDataSourceResultAsync(request);
        }

        public async Task<List<string>> GetUnits()
        {
            return await (from i in _Webcontext.UnitOfMeasures
                          select i.UOM).Distinct().ToListAsync();
        }
        public async Task<List<string>> ReadItemCode([FromForm] long ItemCategoryId)
        {
            return await (from i in _Webcontext.Items
                          where i.ItemCategoryId == ItemCategoryId
                          select i.ItemCode).Distinct().ToListAsync();
        }

        [ValidateAction(Forms.Procurement.Items, Rights.Modify)]
        public async Task<ReturnMessage> SaveItems([FromForm] Item data)
        {
            if (!await data.SaveAsync())
                return SaveError(data.ErrorMessage);

            if (data.Mode == "add")
                _notificationService.Send(Helpers.Notification.POSSparePartsRequest.ItemRegistration, data.Id);

            if (data.Id > 0 && (data.SellingItemUnits != null && data.SellingItemUnits.Count > 0))
            {
                foreach (var item in data.SellingItemUnits)
                {
                    item.ItemId = data.Id;

                    if (!await item.SaveAsync())
                        return SaveError(item.ErrorMessage);
                }
            }

            return MessageWithId("Item saved.", data.Id);
        }

        [ValidateAction(Forms.Procurement.Items, Rights.Modify)]
        public async Task<ReturnMessage> UploadItemImage(IFormFile File, [FromForm] long Id)
        {
            string dirPath = Path.Combine(_commonHelper.GetDocumentRoot(), ImagePath.ItemImage.ToString());
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            string fileName = string.Format("{0}{1}{2}", Id, DateTime.Now.ToString("_ddMMyyyyHHmmss"), Path.GetExtension(File.FileName));
            string imagePath = Path.Combine(dirPath, fileName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
                await File.CopyToAsync(fileStream);

            var existing = await _Webcontext.Items.FindAsync(Id);
            existing.ImagePath = fileName;
            await _Webcontext.SaveChangesAsync();

            return MessageWithId("Item details saved.", Id);
        }

        [ValidateAction(Forms.Procurement.Items, Rights.Delete)]
        public async Task<ReturnMessage> ChangeItemStatus([FromForm] long id)
        {
            Item item = await _Webcontext.Items.FindAsync(id);
            item.Active = !item.Active;

            await EventLog("ITEMS", ActionType.Edit.ToString(), "Items", id, "Status changed from " + item.SKU.ToString() + " " + (item.Active ? "In-Active to Active" : "Active to In-Active"));
            return Message("Item Status changed");
        }

        [ValidateAction(Forms.Procurement.Items, Rights.Delete)]
        public async Task<ReturnMessage> DeleteItem([FromForm] Item data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Item deleted.");
        }

        [ValidateAction(Forms.Procurement.Items, Rights.Approve)]
        public async Task<ReturnMessage> VerifyItem([FromForm] Item data)
        {
            if (!await data.ApproveAsync())
                return ApproveError(data.ErrorMessage);

            return Message("Item verified.");
        }
        [ValidateAction(Forms.Procurement.Items, Rights.Approve)]
        public async Task<ReturnMessage> RevertItem([FromForm] Item data)
        {
            if (!await data.RejectAsync())
                return UndoError(data.ErrorMessage);

            return Message("Item un-verified.");
        }
        public async Task<FileContentResult> ExportItems([FromQuery] int statusFilter)
        {
            DataTable dtData = await _Webcontext.ExecuteDataTableAsync("spHRMS_ExportItems", new
            {
                option = "GetItems",
                statusFilter
            });

            byte[] bytes = new ExcelHelper().Export(dtData, "ExportItems");
            return File(bytes, MediaTypeNames.Application.Octet, "ExportItems.xlsx");
        }
        public async Task<FileContentResult> ExportStock([FromQuery] int statusFilter)
        {
            DataTable dtData = await _Webcontext.ExecuteDataTableAsync("spHRMS_ExportItems", new
            {
                option = "GetStock",
                statusFilter
            });

            byte[] bytes = new ExcelHelper().Export(dtData, "ExportItems");
            return File(bytes, MediaTypeNames.Application.Octet, "ExportOpeningStockItems.xlsx");
        }
        [ValidateAction(Forms.Procurement.Items, Rights.Edit)]
        public async Task<ReturnMessage> UpdateInActiveItem([FromForm] Item data)
        {
            Item exits = await _Webcontext.Items.FindAsync(data.Id);
            exits.Active = false;
            await _Webcontext.SaveChangesAsync();
            await EventLog("ITEM", ActionType.Edit.ToString(), "Item - IN - Active", data.Id, data.ItemCode);
            return Message("Saved Successfully.");
        }
        [ValidateAction(Forms.Procurement.Items, Rights.Edit)]
        public async Task<ReturnMessage> RevertInActiveItem([FromForm] Item data)
        {
            Item exits = await _Webcontext.Items.FindAsync(data.Id);
            exits.Active = true;
            await _Webcontext.SaveChangesAsync();
            await EventLog("ITEM", ActionType.Edit.ToString(), "Item - Active", data.Id, data.ItemCode);
            return Message("Undo Successfully.");
        }
        public async Task<object> ImportItems(IFormFile Item)
        {
            if (!Item.FileName.ToLower().EndsWith(".xlsx"))
                return Error("Invalid format. Please upload xlsx file only.");

            ExcelHelper excel = new ExcelHelper();
            DataTable dtExcelData = excel.ReadDataFromExcel(Item.OpenReadStream());

            if (dtExcelData == null || dtExcelData.Rows.Count == 0)
                return Error("There is no data in Excel");

            List<string> REQUIRED_COLUMNS = new[] { "SKU", "ItemCode","SupplierName", "UPC", "Category","Description", "ItemType","Unit", "CostOfGoods", "SellingPrice", "Size",
                                                    "OpeningStock","OpeningStockDate","Saleable","Inventory","Serialized","Perishable","GSTApplicable","VendorName","Origin","Brand","HSNCode"}.ToList();
            var missingColumns = (from rqrd in REQUIRED_COLUMNS
                                  join xl in dtExcelData.Columns.OfType<DataColumn>() on rqrd equals xl.ColumnName into grp
                                  where !grp.Any()
                                  select rqrd).ToList();

            if (missingColumns.Any())
                return Error("The following columns are missing in Excel Sheet \n\n" + string.Join("\n", missingColumns) + "\n\n Please download the Excel Template before importing data.");

            Func<object, bool> validateStr = (object value) => { return !(value == DBNull.Value || value == null || string.IsNullOrEmpty(value.ToString())); };

            long temp;
            Func<object, bool> validateLong = (object value) => { return value == DBNull.Value || value == null || !long.TryParse(value.ToString(), out temp) || temp == 0; };


            Func<object, decimal> validateDecimal = (object value) => { return value == DBNull.Value || value == null || !decimal.TryParse(value.ToString(), out decimal temp) ? 0 : temp; };

            Func<object, bool> convertToBool = (object value) =>
            {
                if (value == DBNull.Value || value == null)
                    return false;
                if (new[] { "1", "true", "yes" }.Any(x => x == value.ToString().ToLower()))
                    return true;
                return false;
            };

            DateTime dDate;
            Func<object, bool> validateDate = (object value) => { return (value == DBNull.Value || value == null || DateTime.TryParse(value.ToString(), out dDate)); };

            var lstData = dtExcelData.AsEnumerable().Where(x => validateStr(x["SKU"]) || validateStr(x["ItemCode"]));
            if (lstData.Any())
                dtExcelData = lstData.CopyToDataTable();
            else
                return Error("There is no data to import");

            var errors = new List<DetailedValidationModel>();

            var invalidDates = dtExcelData.AsEnumerable().Where(x => !validateDate(x["OpeningStockDate"]) && validateDecimal(x["OpeningStock"]) > 0);
            if (invalidDates.Any())
                errors.AddRange(invalidDates.Select(x => new DetailedValidationModel
                {
                    Category = "Incorrect Opening Stock Date",
                    Message = $"SKU: {x["SKU"]}, Item Code: {x["ItemCode"]}, OpeningStockDate: {x["OpeningStockDate"]}. Recommended format yyyy-MM-dd"
                }));

            var invalidItemCategory = dtExcelData.AsEnumerable().Where(x => !validateStr(x["Category"]));
            if (invalidItemCategory.Any())
                errors.AddRange(invalidItemCategory.Select(x => new DetailedValidationModel
                {
                    Category = "Category should not be empty",
                    Message = $"SKU: {x["SKU"]}, Item Code: {x["ItemCode"]}"
                }));

            var invalidItemCode = dtExcelData.AsEnumerable().Where(x => !validateStr(x["ItemCode"]));
            if (invalidItemCode.Any())
                errors.AddRange(invalidItemCode.Select(x => new DetailedValidationModel
                {
                    Category = "Itemcode should not be empty",
                    Message = $"SKU: {x["SKU"]}"
                }));

            var invalidSKU = dtExcelData.AsEnumerable().Where(x => validateLong(x["SKU"]));
            if (invalidSKU.Any())
                errors.AddRange(invalidSKU.Select(x => new DetailedValidationModel
                {
                    Category = "SKU should not be empty",
                    Message = $"Item Code: {x["ItemCode"]}"
                }));

            var invalidDescription = dtExcelData.AsEnumerable().Where(x => !validateStr(x["Description"]));
            if (invalidDescription.Any())
                errors.AddRange(invalidDescription.Select(x => new DetailedValidationModel
                {
                    Category = "Description should not be empty",
                    Message = $"SKU: {x["SKU"]}, Item Code: {x["ItemCode"]}"
                }));

            var invalidItemType = dtExcelData.AsEnumerable().Where(x => !validateStr(x["ItemType"]));
            if (invalidItemType.Any())
                errors.AddRange(invalidItemType.Select(x => new DetailedValidationModel
                {
                    Category = "ItemType should not be empty",
                    Message = $"SKU: {x["SKU"]}, Item Code: {x["ItemCode"]}"
                }));

            var invalidUnit = dtExcelData.AsEnumerable().Where(x => !validateStr(x["Unit"]));
            if (invalidUnit.Any())
                errors.AddRange(invalidUnit.Select(x => new DetailedValidationModel
                {
                    Category = "Unit should not be empty",
                    Message = $"SKU: {x["SKU"]}, Item Code: {x["ItemCode"]}"
                }));

            var invalidModelCode = dtExcelData.AsEnumerable().Where(x => !validateStr(x["ModelCode"]) && x["ItemType"].ToString() == "Unit");
            if (invalidModelCode.Any())
                errors.AddRange(invalidModelCode.Select(x => new DetailedValidationModel
                {
                    Category = "ModelCode should not be empty",
                    Message = $"SKU: {x["SKU"]}, Item Code: {x["ItemCode"]}"
                }));

            var invalidColor = dtExcelData.AsEnumerable().Where(x => !validateStr(x["Color"]) && x["ItemType"].ToString() == "Unit");
            if (invalidColor.Any())
                errors.AddRange(invalidColor.Select(x => new DetailedValidationModel
                {
                    Category = "Color should not be empty",
                    Message = $"SKU: {x["SKU"]}, Item Code: {x["ItemCode"]}, Model Code: {x["ModelCode"]}"
                }));

            if (errors.Any())
                return new { HasWarning = true, GridData = errors };

            var tableSchema = new List<SqlMetaData>() {
                    new SqlMetaData("ItemCategory", SqlDbType.VarChar, 150),
                    new SqlMetaData("ItemType", SqlDbType.VarChar, 50),
                    new SqlMetaData("SKU", SqlDbType.BigInt),
                    new SqlMetaData("ItemCode", SqlDbType.VarChar, 50),
                    new SqlMetaData("SupplierName", SqlDbType.VarChar, 50),
                    new SqlMetaData("UPC", SqlDbType.BigInt),
                    new SqlMetaData("Unit", SqlDbType.VarChar, 50),
                    new SqlMetaData("Description", SqlDbType.VarChar,-1),
                    new SqlMetaData("Saleable", SqlDbType.Bit),
                    new SqlMetaData("Inventory", SqlDbType.Bit),
                    new SqlMetaData("Perishable", SqlDbType.Bit),
                    new SqlMetaData("Serialized", SqlDbType.Bit),
                    new SqlMetaData("Size", SqlDbType.VarChar,200),
                    new SqlMetaData("CostOfGoods", SqlDbType.Money),
                    new SqlMetaData("SellingPrice", SqlDbType.Money),
                    new SqlMetaData("OpeningStock", SqlDbType.Decimal,10,2),
                    new SqlMetaData("OpeningStockDate", SqlDbType.Date),
                    new SqlMetaData("GSTApplicable", SqlDbType.Bit),
                    new SqlMetaData("VendorName", SqlDbType.VarChar, 50),
                    new SqlMetaData("OriginName", SqlDbType.VarChar, 50),
                    new SqlMetaData("Brand", SqlDbType.VarChar, 50),
                    new SqlMetaData("HSNCode", SqlDbType.VarChar, 50),

                    new SqlMetaData("Surcharge", SqlDbType.Money),
                    new SqlMetaData("ModelCode", SqlDbType.VarChar, 200),
                    new SqlMetaData("Color", SqlDbType.VarChar, 100),
                    new SqlMetaData("Height", SqlDbType.Decimal,10,2),
                    new SqlMetaData("Length", SqlDbType.Decimal,10,2),
                    new SqlMetaData("Width", SqlDbType.Decimal,10,2)
                }.ToArray();

            var sqlDataRecords = new List<SqlDataRecord>();
            foreach (DataRow row in dtExcelData.Rows)
            {
                SqlDataRecord sqlDataRecord = new SqlDataRecord(tableSchema);
                sqlDataRecord.SetString(0, Convert.ToString(row["Category"]));
                sqlDataRecord.SetString(1, Convert.ToString(row["ItemType"]));
                sqlDataRecord.SetInt64(2, row["SKU"] != DBNull.Value ? Convert.ToInt64(row["SKU"]) : 0);
                sqlDataRecord.SetString(3, Convert.ToString(row["ItemCode"]));
                sqlDataRecord.SetString(4, Convert.ToString(row["SupplierName"]));
                if (long.TryParse(Convert.ToString(row["UPC"]), out long upc))
                    sqlDataRecord.SetInt64(5, upc);
                sqlDataRecord.SetString(6, Convert.ToString(row["Unit"]));
                sqlDataRecord.SetString(7, Convert.ToString(row["Description"]));
                sqlDataRecord.SetBoolean(8, convertToBool(row["Saleable"]));
                sqlDataRecord.SetBoolean(9, convertToBool(row["Inventory"]));
                sqlDataRecord.SetBoolean(10, convertToBool(row["Perishable"]));
                sqlDataRecord.SetBoolean(11, convertToBool(row["Serialized"]));
                sqlDataRecord.SetString(12, Convert.ToString(row["Size"]));
                sqlDataRecord.SetDecimal(13, validateDecimal(row["CostOfGoods"]));
                sqlDataRecord.SetDecimal(14, validateDecimal(row["SellingPrice"]));
                sqlDataRecord.SetDecimal(15, validateDecimal(row["OpeningStock"]));
                sqlDataRecord.SetDateTime(16, row["OpeningStockDate"] != DBNull.Value ? Convert.ToDateTime(row["OpeningStockDate"]) : Convert.ToDateTime(null));
                sqlDataRecord.SetBoolean(17, convertToBool(row["GSTApplicable"]));
                sqlDataRecord.SetString(18, Convert.ToString(row["VendorName"]));
                sqlDataRecord.SetString(19, Convert.ToString(row["Origin"]));
                sqlDataRecord.SetString(20, Convert.ToString(row["Brand"]));
                sqlDataRecord.SetString(21, Convert.ToString(row["HSNCode"]));

                sqlDataRecord.SetDecimal(22, validateDecimal(row["Surcharge"]));
                sqlDataRecord.SetString(23, Convert.ToString(row["ModelCode"]));
                sqlDataRecord.SetString(24, Convert.ToString(row["Color"]));
                sqlDataRecord.SetDecimal(25, validateDecimal(row["Height"]));
                sqlDataRecord.SetDecimal(26, validateDecimal(row["Length"]));
                sqlDataRecord.SetDecimal(27, validateDecimal(row["Width"]));

                sqlDataRecords.Add(sqlDataRecord);
            }
            List<SqlParameter> sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@Option", "VAL_IMPORT"),
                new SqlParameter("@UserName", _appUser.CommonEmpNo + " (Imported)"),
                new SqlParameter
                {
                    ParameterName = "@ItemRegistration",
                    Value = sqlDataRecords,
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.ItemsRegistrationType"
                }
            };
            var lstError = await _Webcontext.ExecuteSqlSpAsync<DetailedValidationModel>("spBSOL_AssignItemsForAllShops", sqlParams);
            if (lstError.Any())
                return new { HasWarning = true, GridData = lstError };

            sqlParams[0].Value = "IMPORT";
            var retVal = await _Webcontext.ExecuteSqlNonQueryAsync("spBSOL_AssignItemsForAllShops", sqlParams);

            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            return Message("Item imported successfully");
        }

        public async Task<object> UpdateOpeningStockItems(IFormFile UpdateItem)
        {
            if (!UpdateItem.FileName.ToLower().EndsWith(".xlsx"))
                return Error("Invalid format. Please upload xlsx file only.");

            ExcelHelper excel = new ExcelHelper();
            DataTable dtExcelData = excel.ReadDataFromExcel(UpdateItem.OpenReadStream());

            if (dtExcelData == null || dtExcelData.Rows.Count == 0)
                return Error("There is no data in Excel");

            List<string> REQUIRED_COLUMNS = new[] { "SKU" }.ToList();

            var missingColumns = (from rqrd in REQUIRED_COLUMNS
                                  join xl in dtExcelData.Columns.OfType<DataColumn>() on rqrd equals xl.ColumnName into grp
                                  where !grp.Any()
                                  select rqrd).ToList();

            if (missingColumns.Any())
                return Error("The following columns are missing in Excel Sheet \n\n" + string.Join("\n", missingColumns) + "\n\n Please download the Excel Template before importing data.");

            Func<object, bool> validateStr = (object value) => { return !(value == DBNull.Value || value == null || string.IsNullOrEmpty(value.ToString())); };

            long temp;
            Func<object, bool> validateLong = (object value) => { return value == DBNull.Value || value == null || !long.TryParse(value.ToString(), out temp) || temp == 0; };


            Func<object, float> validatefloat = (object value) => { return value == DBNull.Value || value == null || !float.TryParse(value.ToString(), out float temp) ? 0 : temp; };

            Func<object, bool> convertToBool = (object value) =>
            {
                if (value == DBNull.Value || value == null)
                    return false;
                if (new[] { "1", "true", "yes" }.Any(x => x == value.ToString().ToLower()))
                    return true;
                return false;
            };

            DateTime dDate;
            Func<object, bool> validateDate = (object value) => { return (value == DBNull.Value || value == null || DateTime.TryParse(value.ToString(), out dDate)); };

            var lstData = dtExcelData.AsEnumerable().Where(x => validateStr(x["SKU"]));
            if (lstData.Any())
                dtExcelData = lstData.CopyToDataTable();
            else
                return Error("There is no data to import");

            var errors = new List<DetailedValidationModel>();

            var invalidDates = dtExcelData.AsEnumerable().Where(x => !validateDate(x["OpeningStockDate"]));
            if (invalidDates.Any())
                errors.AddRange(invalidDates.Select(x => new DetailedValidationModel
                {
                    Category = "Incorrect Opening Stock Date",
                    Message = $"SKU: {x["SKU"]}, OpeningStockDate: {x["OpeningStockDate"]}. Recommended format yyyy-MM-dd"
                }));

            var invalidSKU = dtExcelData.AsEnumerable().Where(x => validateLong(x["SKU"]));
            if (invalidSKU.Any())
                errors.AddRange(invalidSKU.Select(x => new DetailedValidationModel
                {
                    Category = "SKU should not be empty",
                    Message = $"SKU: {x["SKU"]}"
                }));

            if (errors.Any())
                return new { HasWarning = true, GridData = errors };

            var tableSchema = new List<SqlMetaData>() {
                    new SqlMetaData("SKU", SqlDbType.BigInt),
                    new SqlMetaData("OpeningStock", SqlDbType.Float),
                    new SqlMetaData("OpeningStockDate", SqlDbType.Date),
                }.ToArray();

            var sqlDataRecords = new List<SqlDataRecord>();
            foreach (DataRow row in dtExcelData.Rows)
            {
                SqlDataRecord sqlDataRecord = new SqlDataRecord(tableSchema);
                sqlDataRecord.SetInt64(0, Convert.ToInt64(row["SKU"]));
                sqlDataRecord.SetDouble(1, Convert.ToDouble(row["OpeningStock"]));
                sqlDataRecord.SetDateTime(2, Convert.ToDateTime(row["OpeningStockDate"]));
                sqlDataRecords.Add(sqlDataRecord);
            }
            List<SqlParameter> sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@Option", "VAL_UPDATE_IMPORT"),
                new SqlParameter("@UserName", _appUser.CommonEmpNo + " (Imported)"),
                new SqlParameter
                {
                    ParameterName = "@OpeningStockUpdate",
                    Value = sqlDataRecords,
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.OpeningStockUpdateType"
                }
            };

            sqlParams[0].Value = "OPENING_STOCK_UPDATE";
            var retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_ImportOpeningStock", sqlParams);

            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            return Message("Item imported successfully");
        }
        public async Task<DataSourceResult> ReadSupplierCode([DataSourceRequest] DataSourceRequest request, [FromForm] long itemId)
        {
            var result = (from si in _Webcontext.SupplierItemCodes
                          join i in _Webcontext.Items on si.ItemId equals i.Id
                          join s in _Webcontext.Suppliers on si.SupplierId equals s.Id
                          where si.ItemId == itemId && s.Active
                          select new SupplierCode
                          {
                              SupplierItemCode = si.SupplierItemCode,
                              ItemId = i.Id,
                              SupplierName = s.SupplierName
                          });
            return await result.ToDataSourceResultAsync(request);
        }
        #endregion
        #region Item Categories
        [ValidateAction(Forms.Procurement.Items, Rights.View)]
        public async Task<DataSourceResult> ReadItemCategories([DataSourceRequest] DataSourceRequest request)
        {
            var result = await _Webcontext.ItemCategories.Where(x => x.CompanyId == _appUser.CompanyId).ToListAsync();
            return result.ToDataSourceResult(request);
        }

        [ValidateAction(Forms.Procurement.Items, Rights.Modify)]
        public async Task<DataSourceResult> UpdateItemCategories([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "models")] IEnumerable<ItemCategory> datas)
        {
            foreach (var item in datas)
            {
                item.CompanyId = _appUser.CompanyId;
                if (!await item.SaveAsync())
                    ModelState.AddModelError(item.ErrorMessage);
            }
            return datas.ToDataSourceResult(request, ModelState);
        }

        [ValidateAction(Forms.Procurement.Items, Rights.Delete)]
        public async Task<ReturnMessage> DeleteItemCategory([FromForm] ItemCategory data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Item Category deleted.");
        }
        #endregion
        #region Sales Categories
        [ValidateAction(Forms.Procurement.Items, Rights.View)]
        public async Task<DataSourceResult> ReadSalesCategories([DataSourceRequest] DataSourceRequest request)
        {
            var result = await _Webcontext.SalesCategories.Where(x => x.CompanyId == _appUser.CompanyId).ToListAsync();
            return result.ToDataSourceResult(request);
        }

        [ValidateAction(Forms.Procurement.Items, Rights.Modify)]
        public async Task<DataSourceResult> UpdateSalesCategories([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "models")] IEnumerable<SalesCategory> datas)
        {
            foreach (var item in datas)
            {
                item.CompanyId = _appUser.CompanyId;
                if (!await item.SaveAsync())
                    ModelState.AddModelError(item.ErrorMessage);
            }
            return datas.ToDataSourceResult(request, ModelState);
        }

        [ValidateAction(Forms.Procurement.Items, Rights.Delete)]
        public async Task<ReturnMessage> DeleteSalesCategory([FromForm] SalesCategory data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Sales Category deleted.");
        }
        #endregion
        #region UnitOfMeasures
        [ValidateAction(Forms.Procurement.Items, Rights.View)]
        public async Task<DataSourceResult> ReadUnitOfMeasures([DataSourceRequest] DataSourceRequest request)
        {
            var result = await (from s in _Webcontext.UnitOfMeasures
                                select new
                                {
                                    s.ID,
                                    s.UOM
                                }).OrderByDescending(x => x.ID).ToListAsync();
            return result.ToDataSourceResult(request);
        }
        [ValidateAction(Forms.Procurement.Items, Rights.Modify)]
        public async Task<DataSourceResult> UpdateUnitOfMeasures([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "Models")] IEnumerable<UnitOfMeasures> data)
        {
            foreach (var item in data)
            {
                if (!await item.SaveAsync())
                    ModelState.AddModelError(item.ErrorMessage);
            }
            return data.ToDataSourceResult(request, ModelState);
        }
        [ValidateAction(Forms.Procurement.Items, Rights.Delete)]
        public async Task<ReturnMessage> DeleteUnit([FromForm] UnitOfMeasures data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Unit Of Measures deleted.");
        }

        #endregion
        #region Suppliers
        [HttpPost]
        [ValidateAction(Forms.Procurement.Supplier, Rights.View)]
        public async Task<DataSourceResult> ReadSuppliers([DataSourceRequest] DataSourceRequest request, [FromForm] int status)
        {
            var result = await (from sp in _Webcontext.Suppliers
                                join lsbd in _Webcontext.SupplierBankDetails on new { sp.Id, IsPrimary = true, Active = true } equals new { Id = lsbd.SupplierId, lsbd.IsPrimary, lsbd.Active } into grpSBD
                                from sbd in grpSBD.DefaultIfEmpty()
                                where sp.CompanyId == _appUser.CompanyId && ((status == 0 && sp.Active) || (status == 1 && !sp.Active))
                                select new Supplier
                                {
                                    Id = sp.Id,
                                    CompanyId = sp.CompanyId,
                                    SupplierName = sp.SupplierName,
                                    SupplierCode = sp.SupplierCode,
                                    RegNo = sp.RegNo,
                                    GSTIN = sp.GSTIN,
                                    Country = sp.Country,
                                    State = sp.State,
                                    City = sp.City,
                                    Address = sp.Address,
                                    PhoneNo = sp.PhoneNo,
                                    Email = sp.Email,
                                    Website = sp.Website,
                                    Notes = sp.Notes,
                                    OpeningBalance = sp.OpeningBalance,
                                    OBDate = sp.OBDate,
                                    EntryBy = sp.EntryBy,
                                    EntryDate = sp.EntryDate,
                                    GST = sp.GST,
                                    Active = sp.Active,
                                    Type = sp.Type,
                                    ZipCode = sp.ZipCode,
                                    ServiceIds = (from ic in _Webcontext.ItemCategories
                                                  join ss in _Webcontext.SupplierServiceDetails on ic.Id equals ss.ItemCategoryId
                                                  where ss.SupplierId == sp.Id
                                                  select ss.ItemCategoryId
                                                  ).ToList(),
                                    ServiceIdlsts = string.Join("/", (from ic in _Webcontext.ItemCategories
                                                                      join ss in _Webcontext.SupplierServiceDetails on ic.Id equals ss.ItemCategoryId
                                                                      where ss.SupplierId == sp.Id
                                                                      select ic.Category
                                                    ).ToList()),
                                    Currency = sp.Currency,
                                    VendorGroup = sp.VendorGroup,
                                    DuePeriodDays = sp.DuePeriodDays,
                                    BankName = sbd.BankName,
                                    AccountName = sbd.AccountName,
                                    AccountNo = sbd.AccountNo,
                                    Branch = sbd.Branch,
                                    SwiftCode = sbd.SwiftCode,
                                    ChequeAccountName = sp.ChequeAccountName
                                }).ToListAsync();

            return result.ToDataSourceResult(request);
        }
        [HttpPost]
        [ValidateAction(Forms.Procurement.Supplier, Rights.Modify)]
        public async Task<ReturnMessage> SaveSupplier([FromForm] Supplier data, [FromForm] string SupplierCategoryIds)
        {
            data.CompanyId = _appUser.CompanyId;
            if (!await data.SaveAsync())
                return SaveError(data.ErrorMessage);

            if (data.Id > 0 && SupplierCategoryIds != "")
            {
                int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_SupplierServiceList", new { Option = "UPDATE", SupplierId = data.Id, SupplierCategoryIds });
            }
            return Message("Supplier details saved.", data.Id);
        }
        [HttpPost]
        [ValidateAction(Forms.Procurement.Supplier, Rights.Delete)]
        public async Task<ReturnMessage> DeleteSupplier([FromForm] Supplier data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Supplier details deleted.");
        }
        [ValidateAction(Forms.Procurement.Supplier, Rights.Delete)]
        public async Task<ReturnMessage> UndoSupplier([FromForm] Supplier data)
        {
            if (!await data.RejectAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Undo Successfully.");
        }
        #endregion

        #region Supplier Contacts
        [HttpPost]
        [ValidateAction(Forms.Procurement.Supplier, Rights.Modify)]
        public async Task<DataSourceResult> ReadSupplierContacts([DataSourceRequest] DataSourceRequest request, [FromForm] long SupplierId)
        {
            var result = await _Webcontext.SupplierContacts.Where(x => x.SupplierId == SupplierId).ToListAsync();
            return result.ToDataSourceResult(request);
        }
        [HttpPost]

        [ValidateAction(Forms.Procurement.Supplier, Rights.Modify)]
        public async Task<DataSourceResult> UpdateSupplierContact([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "models")] IEnumerable<SupplierContact> datas, [FromForm] long SupplierId)
        {
            foreach (var item in datas)
            {
                item.SupplierId = SupplierId;
                if (!await item.SaveAsync())
                    ModelState.AddModelError(item.ErrorMessage);
            }

            return datas.ToDataSourceResult(request, ModelState);
        }

        [HttpPost]
        [ValidateAction(Forms.Procurement.Supplier, Rights.Delete)]
        public async Task<ReturnMessage> DeleteSupplierContact([FromForm] long Id)
        {
            SupplierContact data = new SupplierContact();
            data.Id = Id;
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Contact deleted.");
        }
        #endregion

        #region Supplier Bank Details
        [HttpPost]
        public async Task<List<DropDownModel>> ReadBank()
        {
            return await _Webcontext.Banks.Select(x => new DropDownModel { Id = x.ID, Text = x.BankName }).ToListAsync();
        }
        [HttpPost]
        [ValidateAction(Forms.Procurement.Supplier, Rights.Modify)]
        public async Task<DataSourceResult> ReadSupplierBankDetails([DataSourceRequest] DataSourceRequest request, [FromForm] long SupplierId)
        {
            var result = await (from sb in _Webcontext.SupplierBankDetails
                                join bk in _Webcontext.Banks on sb.BankId equals bk.ID
                                where sb.SupplierId == SupplierId && sb.Active
                                select new
                                {
                                    sb.ID
                                   ,
                                    sb.BankId
                                   ,
                                    sb.BankName
                                   ,
                                    sb.AccountName
                                   ,
                                    sb.AccountNo
                                   ,
                                    sb.Branch
                                   ,
                                    sb.SwiftCode
                                   ,
                                    sb.SupplierId
                                   ,
                                    sb.Active
                                    ,
                                    sb.Address
                                    ,
                                    sb.IsPrimary
                                }
                               ).ToListAsync();
            return result.ToDataSourceResult(request);
        }
        [HttpPost]

        [ValidateAction(Forms.Procurement.Supplier, Rights.Modify)]
        public async Task<DataSourceResult> UpdateSupplierBankDetails([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "models")] IEnumerable<SupplierBankDetail> datas, [FromForm] long SupplierId)
        {
            foreach (var item in datas)
            {
                item.Active = true;
                item.SupplierId = SupplierId;
                if (!await item.SaveAsync())
                    ModelState.AddModelError(item.ErrorMessage);
            }

            return datas.ToDataSourceResult(request, ModelState);
        }

        [HttpPost]
        [ValidateAction(Forms.Procurement.Supplier, Rights.Delete)]
        public async Task<ReturnMessage> DeleteSupplierBankDetails([FromForm] long Id)
        {
            SupplierBankDetail data = new SupplierBankDetail();
            data.ID = Id;
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Bank details deleted.");
        }
        #endregion

        #region Shops

        #region Shop Details
        [ValidateAction(Forms.Procurement.Shops, Rights.View)]
        public async Task<DataSourceResult> ReadShops([DataSourceRequest] DataSourceRequest request)
        {
            var result = await (from s in _Webcontext.Shops
                                join sg in _Webcontext.ShopGroups on s.ShopGroupID equals sg.ID
                                join gst in _Webcontext.GSTSettings on s.GSTSettingId equals gst.Id into grpG
                                from g in grpG.DefaultIfEmpty()
                                select new
                                {
                                    s.Id,
                                    s.CompanyId,
                                    s.ShopName,
                                    s.ShopCode,
                                    s.ShortName,
                                    s.GST,
                                    s.GSTSettingId,
                                    GSTSettingName = s.GSTSettingId == null ? "" : g.Name,
                                    s.ServiceCharge,
                                    s.Created,
                                    s.CreatedOn,
                                    s.Verified,
                                    s.VerifiedOn,
                                    s.Stock,
                                    Status = !string.IsNullOrEmpty(s.Verified) ? 1 : 0,
                                    s.Active,
                                    s.ShopGroupID,
                                    sg.Name,
                                    s.MovingAmount,
                                    s.FloatAmountMVR,
                                    s.OpeningFloatMVR,
                                    s.OpeningFloatUSD,
                                    s.NoOfCounter
                                }).OrderBy(x => x.Status).ToListAsync();
            return result.ToDataSourceResult(request);
        }
        [ValidateAction(Forms.Procurement.Shops, Rights.Modify)]
        public async Task<DataSourceResult> UpdateShopDetails([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "Models")] IEnumerable<Shop> data)
        {
            foreach (var item in data)
            {
                item.CompanyId = _appUser.CompanyId;
                item.ShortName = string.IsNullOrEmpty(item.ShortName) ? item.ShopCode : item.ShortName;
                if (!await item.SaveAsync())
                    ModelState.AddModelError(item.ErrorMessage);
            }
            return data.ToDataSourceResult(request, ModelState);
        }
        [ValidateAction(Forms.Procurement.Shops, Rights.Delete)]
        public async Task<ReturnMessage> DeleteShopDetails([FromForm] Shop data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Shop deleted.");
        }
        [ValidateAction(Forms.Procurement.Shops, Rights.Delete)]
        public async Task<ReturnMessage> UndoShopDetails([FromForm] Shop data)
        {
            var existing = await _Webcontext.Shops.FindAsync(data.Id);
            _Webcontext.Entry(existing).Property(x => x.Active).CurrentValue = true;

            await _Webcontext.SaveChangesAsync();
            return Message("Shop reverted.");
        }

        [ValidateAction(Forms.Procurement.Shops, Rights.Approve)]
        public async Task<ReturnMessage> VerifyShop([FromForm] Shop data)
        {
            if (!await data.ApproveAsync())
                return ApproveError(data.ErrorMessage);

            return Message("Shop verified.");
        }
        [ValidateAction(Forms.Procurement.Shops, Rights.Approve)]
        public async Task<ReturnMessage> RevertShop([FromForm] Shop data)
        {
            if (!await data.RejectAsync())
                return ApproveError(data.ErrorMessage);

            return Message("Shop reverted.");
        }
        #endregion

        #region Shop Group
        [ValidateAction(Forms.Procurement.Shops, Rights.Modify)]
        public async Task<DataSourceResult> UpdateShopGroup([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "Models")] IEnumerable<ShopGroup> data)
        {
            foreach (var item in data)
            {
                if (!await item.SaveAsync())
                    ModelState.AddModelError(item.ErrorMessage);
            }
            return data.ToDataSourceResult(request, ModelState);
        }
        [ValidateAction(Forms.Procurement.Shops, Rights.Delete)]
        public async Task<ReturnMessage> DeleteShopGroupDetails([FromForm] ShopGroup data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Shop Group deleted.");
        }
        [ValidateAction(Forms.Procurement.Shops, Rights.View)]
        public async Task<DataSourceResult> ReadShopGroup([DataSourceRequest] DataSourceRequest request)
        {
            var result = await (from sg in _Webcontext.ShopGroups
                                select new
                                {
                                    sg.ID,
                                    sg.Name,
                                    sg.EntryBy,
                                    sg.EntryOn,
                                    sg.GSTIN
                                }).ToListAsync();
            return result.ToDataSourceResult(request);
        }
        #endregion
        #region ShopCounters 

        [ValidateAction(Forms.Procurement.Shops, Rights.View)]
        public async Task<DataSourceResult> ReadShopCounters([DataSourceRequest] DataSourceRequest request)
        {
            var result = (from shopCounters in _Webcontext.ShopCounters
                          join shops in _Webcontext.Shops on shopCounters.ShopId equals shops.Id
                          select new ShopCounter
                          {
                              Id = shopCounters.Id,
                              ShopId = shopCounters.ShopId,
                              ShopName = shops.ShopName,
                              CounterName = shopCounters.CounterName,
                              NoOfCounter = shopCounters.NoOfCounter
                          });
            return await result.ToDataSourceResultAsync(request);
        }
        [ValidateAction(Forms.Procurement.Shops, Rights.Modify)]
        public async Task<DataSourceResult> UpdateShopCounters([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "Models")] IEnumerable<ShopCounter> datas)
        {
            foreach (var shopCounters in datas)
            {
                if (!await shopCounters.SaveAsync())
                    ModelState.AddModelError(shopCounters.ErrorMessage);
            }
            return datas.ToDataSourceResult(request, ModelState);
        }

        [ValidateAction(Forms.Procurement.Shops, Rights.Delete)]
        public async Task<ReturnMessage> DeleteShopCounters([FromForm] ShopCounter data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Shop deleted.");
        }
        #endregion
        #endregion
        #region Purchase

        #region PurchaseOrder
        [ValidateAction(Forms.Procurement.Purchase, Rights.View)]
        public async Task<DataSourceResult> ReadPurchaseOrder([DataSourceRequest] DataSourceRequest request, int ApprovedStatus = 0)
        {
            var result = (from PO in _Webcontext.PurchaseOrders
                          join S in _Webcontext.Suppliers on PO.SupplierId equals S.Id
                          join SC in _Webcontext.SupplierContacts on new { SupplierId = S.Id, IsPrimaryContact = true } equals new { SC.SupplierId, SC.IsPrimaryContact } into grpSC
                          from C in grpSC.DefaultIfEmpty()
                          join Sg in _Webcontext.ShopGroups on PO.ShopGroupID equals Sg.ID into grpSG
                          from lsg in grpSG.DefaultIfEmpty()
                          where ApprovedStatus == 0 ? PO.VerifiedOn == null && PO.CheckedOn == null && PO.Active :
                                ApprovedStatus == 1 ? PO.CheckedOn != null && PO.VerifiedOn == null && PO.Active :
                                ApprovedStatus == 2 ? PO.CheckedOn != null && PO.VerifiedOn != null && PO.Active : !PO.Active
                          orderby PO.Id descending
                          select new PurchaseOrder
                          {
                              Id = PO.Id,
                              RefNoFormatted = PO.RefNoFormatted,
                              PurchaseOrderDate = PO.PurchaseOrderDate,
                              TotalAmount = PO.TotalAmount,
                              DiscountAmount = PO.DiscountAmount,
                              GSTAmount = PO.GSTAmount,
                              Mode = PO.Mode,
                              Currency = PO.Currency,
                              VerifiedBy = PO.VerifiedBy,
                              VerifiedOn = PO.VerifiedOn,
                              CreatedBy = PO.CreatedBy,
                              CreatedOn = PO.CreatedOn,
                              SupplierName = S.SupplierName,
                              ContactName = C.ContactName,
                              ContactNo = C.ContactNo,
                              ClosedBy = PO.ClosedBy,
                              ClosedOn = PO.ClosedOn,
                              IncoTerm = PO.IncoTerm,
                              ShopGroupID = PO.ShopGroupID,
                              ShopGroupName = lsg.Name,
                              ReasonForCancel = PO.ReasonForCancel,
                              CheckedBy = PO.CheckedBy,
                              CheckedOn = PO.CheckedOn,
                              SupplierId = PO.SupplierId,
                              PaymentType = PO.PaymentType,
                              Notes = PO.Notes,
                              Remarks = PO.Remarks,
                              OtherRefNo = PO.OtherRefNo != null ? PO.OtherRefNo : PO.QuotationNo,
                              PaymentRefNo = (from pr in _Webcontext.PaymentRequest
                                              join pd in _Webcontext.PaymentDetails on pr.ID equals pd.PaymentMasterId
                                              where pd.ReferenceId == PO.Id && pd.Reference == "PURCHASE ORDER" && pr.Active == true
                                              select pr.RefNoFormatted).FirstOrDefault(),
                              RemainingBalance = PO.TotalAmount - (
                                                from pr in _Webcontext.PaymentRequest
                                                join pd in _Webcontext.PaymentDetails on pr.ID equals pd.PaymentMasterId
                                                where pd.ReferenceId == PO.Id && pd.Reference == "PURCHASE ORDER" && pr.Active == true
                                                select pd.Amount + pd.GSTAmount).Sum()
                          });
            return await result.ToDataSourceResultAsync(request);
        }

        [ValidateAction(Forms.Procurement.Purchase, Rights.Modify)]
        public async Task<PurchaseOrder> GetPurchaseOrderDetails([ModelBinder(typeof(IdentityDecryptor))] long id)
        {
            return await (from po in _Webcontext.PurchaseOrders
                          join s in _Webcontext.Suppliers on po.SupplierId equals s.Id
                          join Sg in _Webcontext.ShopGroups on po.ShopGroupID equals Sg.ID into grpSG
                          from lsg in grpSG.DefaultIfEmpty()
                          where po.Id == id && s.Active
                          select new PurchaseOrder
                          {
                              Id = po.Id,
                              RefNo = po.RefNo,
                              SupplierId = po.SupplierId,
                              PaymentType = po.PaymentType,
                              PurchaseOrderDate = po.PurchaseOrderDate,
                              Currency = po.Currency,
                              ConversionRate = po.ConversionRate,
                              DiscountPercent = po.DiscountPercent,
                              DiscountAmount = po.DiscountAmount,
                              GSTPercent = po.GSTPercent,
                              GSTAmount = po.GSTAmount,
                              TotalAmount = po.TotalAmount,
                              Balance = po.Balance,
                              Remarks = po.Remarks,
                              Notes = po.Notes,
                              Mode = po.Mode,
                              Active = po.Active,
                              VerifiedBy = po.VerifiedBy,
                              VerifiedOn = po.VerifiedOn,
                              CreatedBy = po.CreatedBy,
                              CreatedOn = po.CreatedOn,
                              RefNoFormatted = po.RefNoFormatted,
                              SupplierName = s.SupplierName,
                              IncoTerm = po.IncoTerm,
                              ShopGroupID = po.ShopGroupID,
                              ShopGroupName = lsg.Name,
                              QuotationDate = po.QuotationDate,
                              QuotationNo = po.QuotationNo
                          }).FirstOrDefaultAsync();
        }

        [ValidateAction(Forms.Procurement.Purchase, Rights.View)]
        public async Task<DataSourceResult> ReadPurchaseOrderItems([DataSourceRequest] DataSourceRequest request, [ModelBinder(typeof(IdentityDecryptor))] long PurchaseOrderId)
        {
            var result = await (from pod in _Webcontext.PurchaseOrderDetails
                                join im in _Webcontext.Items on pod.ItemId equals im.Id
                                join ic in _Webcontext.ItemCategories on im.ItemCategoryId equals ic.Id
                                join po in _Webcontext.PurchaseOrders on pod.PurchaseOrderId equals po.Id
                                join s in _Webcontext.SupplierItemCodes on new { ItemId = im.Id, po.SupplierId } equals new { s.ItemId, s.SupplierId } into grpS
                                from si in grpS.DefaultIfEmpty()
                                where pod.PurchaseOrderId == PurchaseOrderId
                                select new PurchaseOrderDetail
                                {
                                    Id = pod.Id,
                                    PurchaseOrderId = pod.PurchaseOrderId,
                                    ItemId = pod.ItemId,
                                    Description = pod.Description,
                                    BaseRate = pod.BaseRate,
                                    Rate = pod.Rate,
                                    RequestedQty = pod.ActualRequestedQty,
                                    ConfirmedQty = (decimal)pod.ActualConfirmedQty,
                                    TotalPrice = pod.TotalPrice,
                                    Remarks = pod.Remarks,
                                    /* Additional Properties */
                                    SKU = im.SKU,
                                    UPC = im.UPC,
                                    ItemCode = im.ItemCode,
                                    SupplierCode = si.SupplierItemCode,
                                    ItemDescription = im.Description,
                                    ItemCategory = ic.Category,
                                    ImagePath = im.ImagePath,
                                    GSTApplicable = im.GSTApplicable,
                                    PurchaseUnit = pod.PurchaseUnit,
                                    Conversion = pod.Conversion,
                                    PurchaseUnits = (from siu in _Webcontext.SellingItemUnits
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
                                    Size = im.Size,
                                    Unit = im.Unit
                                }).ToListAsync();
            return result.OrderBy(x => x.Id).Select((x, i) => new
            {
                SNO = i + 1,
                x.Id,
                x.PurchaseOrderId,
                x.ItemId,
                x.Description,
                x.BaseRate,
                x.Rate,
                x.RequestedQty,
                x.ConfirmedQty,
                x.TotalPrice,
                x.Remarks,
                x.SKU,
                x.UPC,
                x.ItemCode,
                x.SupplierCode,
                x.ItemDescription,
                x.ItemCategory,
                x.ImagePath,
                x.GSTApplicable,
                x.PurchaseUnit,
                x.Conversion,
                x.PurchaseUnits,
                x.Size,
                x.Unit
            }).OrderByDescending(x => x.SNO).ToDataSourceResult(request);
        }

        [ValidateAction(Forms.Procurement.Purchase, Rights.Modify)]
        public async Task<ReturnMessage> SavePurchaseOrder([FromForm] PurchaseOrder purchaseOrder, [FromForm] List<PurchaseOrderDetail> purchaseOrderDetails)
        {
            var tableSchema = new List<SqlMetaData>() {
                new SqlMetaData("Id", SqlDbType.BigInt),
                new SqlMetaData("ItemId", SqlDbType.BigInt),
                new SqlMetaData("Description", SqlDbType.VarChar, -1),
                new SqlMetaData("BaseRate", SqlDbType.Money),
                new SqlMetaData("Rate", SqlDbType.Money),
                new SqlMetaData("RequestedQty", SqlDbType.Decimal,18,2),
                new SqlMetaData("TotalPrice", SqlDbType.Money),
                new SqlMetaData("Remarks", SqlDbType.VarChar, -1),
                new SqlMetaData("PurchaseUnit", SqlDbType.VarChar, -1),
                new SqlMetaData("Conversion", SqlDbType.Decimal,18,2),
            }.ToArray();
            var sqlDataRecords = new List<SqlDataRecord>();
            purchaseOrderDetails.OrderBy(x => x.SNO).ToList().ForEach(x =>
            {
                SqlDataRecord sqlDataRecord = new SqlDataRecord(tableSchema);
                sqlDataRecord.SetInt64(0, x.Id);
                sqlDataRecord.SetInt64(1, x.ItemId);
                sqlDataRecord.SetString(2, x.Description ?? "");
                sqlDataRecord.SetDecimal(3, x.BaseRate);
                sqlDataRecord.SetDecimal(4, x.Rate);
                sqlDataRecord.SetDecimal(5, x.RequestedQty);
                sqlDataRecord.SetDecimal(6, x.TotalPrice);
                sqlDataRecord.SetString(7, x.Remarks ?? "");
                sqlDataRecord.SetString(8, x.PurchaseUnit ?? "");
                sqlDataRecord.SetDecimal(9, x.Conversion);
                sqlDataRecords.Add(sqlDataRecord);
            });

            List<SqlParameter> sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@Option", "VAL_SAVE"),
                new SqlParameter("@Id", purchaseOrder.Id),
                new SqlParameter("@SupplierId", purchaseOrder.SupplierId),
                new SqlParameter("@PaymentType", purchaseOrder.PaymentType),
                new SqlParameter("@PurchaseOrderDate", purchaseOrder.PurchaseOrderDate),
                new SqlParameter("@Currency", purchaseOrder.Currency),
                new SqlParameter("@ConversionRate", purchaseOrder.ConversionRate),
                new SqlParameter("@DiscountPercent", purchaseOrder.DiscountPercent),
                new SqlParameter("@DiscountAmount", purchaseOrder.DiscountAmount),
                new SqlParameter("@GSTPercent", purchaseOrder.GSTPercent),
                new SqlParameter("@GSTAmount", purchaseOrder.GSTAmount),
                new SqlParameter("@TotalAmount", purchaseOrder.TotalAmount),
                new SqlParameter("@Balance", purchaseOrder.Balance),
                new SqlParameter("@Remarks", purchaseOrder.Remarks),
                new SqlParameter("@Notes", purchaseOrder.Notes),
                new SqlParameter("@Mode", purchaseOrder.Mode),
                new SqlParameter("@CreatedBy", _appUser.CommonEmpNo),
                new SqlParameter("@IncoTerm", purchaseOrder.IncoTerm),
                new SqlParameter("@ShopGroupID", purchaseOrder.ShopGroupID),
                new SqlParameter("@QuotationNo", purchaseOrder.QuotationNo),
                new SqlParameter("@QuotationDate", purchaseOrder.QuotationDate),
                new SqlParameter
                {
                    ParameterName = "@PurchaseOrderDetails",
                    Value = sqlDataRecords,
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.PurchaseOrderDetailType"
                }
            };

            var errors = await _Webcontext.ValidateSqlAsync("spBSOL_PurchaseOrder", sqlParams);
            if (errors.Any())
                return SaveError(errors);
            sqlParams[0].Value = purchaseOrder.Id > 0 ? "UPDATE" : "INSERT";

            long id = await _Webcontext.ExecuteSqlNonQueryWithReturnAsync<long>("spBSOL_PurchaseOrder", sqlParams);
            if (id <= 0)
                return Error(Constants.DBErrorMessage);

            if (purchaseOrder.Id > 0)
                await EventLog("PURCHASE ORDER", ActionType.Edit.ToString(), "PurchaseOrder", purchaseOrder.Id, purchaseOrder.RefNoFormatted);
            else
                _notificationService.Send(Helpers.Notification.POSSparePartsRequest.ItemRegistration, purchaseOrder.Id);

            return MessageWithEncryptedID("Purchase order saved.", id);
        }

        [ValidateAction(Forms.Procurement.Purchase, Rights.Delete)]
        public async Task<ReturnMessage> DeletePurchaseOrder([ModelBinder(typeof(IdentityDecryptor))] long Id, [FromForm] string RefNo)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_PurchaseOrder", new { Option = "VAL_DELETE", Id });
            if (errors.Any())
                return DeleteError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_PurchaseOrder", new { Option = "DELETE", Id });
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await new Document() { DocumentRoot = _commonHelper.GetDocumentRoot() }.DeleteByReference(DocumentReference.PurchaseOrders, Id.ToString());

            await EventLog("PURCHASE ORDER", ActionType.Delete.ToString(), "PurchaseOrder", Id, RefNo);
            return Message("Purchase order deleted.");
        }

        [ValidateAction(Forms.Procurement.Purchase, Rights.Approve)]
        public async Task<ReturnMessage> VerifyPurchaseOrder([ModelBinder(typeof(IdentityDecryptor))] long Id, [FromForm] string RefNo)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_PurchaseOrder", new { Option = "VAL_VERIFY", Id, VerifiedBy = _appUser.CommonEmpNo });
            if (errors.Any())
                return ApproveError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_PurchaseOrder", new { Option = "VERIFY", Id, VerifiedBy = _appUser.CommonEmpNo });
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("PURCHASE ORDER", ActionType.Approve.ToString(), "PurchaseOrder", Id, RefNo);
            return Message("Purchase order Approved.");
        }
        [ValidateAction(Forms.Procurement.Purchase, Rights.Approve)]
        public async Task<ReturnMessage> CheckPurchaseOrder([ModelBinder(typeof(IdentityDecryptor))] long Id, [FromForm] string RefNo)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_PurchaseOrder", new { Option = "VAL_CHECK", Id, VerifiedBy = _appUser.CommonEmpNo });
            if (errors.Any())
                return ApproveError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_PurchaseOrder", new { Option = "CHECK", Id, VerifiedBy = _appUser.CommonEmpNo, _appUser.CompanyId });
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("PURCHASE ORDER", "Check", "PurchaseOrder", Id, RefNo);
            return Message("Purchase order Checked.");
        }

        [ValidateAction(Forms.Procurement.Purchase, Rights.Approve)]
        public async Task<ReturnMessage> UnVerifyPurchaseOrder([ModelBinder(typeof(IdentityDecryptor))] long Id, [FromForm] string RefNo)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_PurchaseOrder", new { Option = "VAL_UN_VERIFY", Id, VerifiedBy = _appUser.CommonEmpNo });
            if (errors.Any())
                return UndoError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_PurchaseOrder", new { Option = "UN_VERIFY", Id });
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("PURCHASE ORDER", ActionType.Reject.ToString(), "PurchaseOrder", Id, RefNo);
            return Message("Purchase order Un-Approved.");
        }
        [ValidateAction(Forms.Procurement.Purchase, Rights.Approve)]
        public async Task<ReturnMessage> UnCheckPurchaseOrder([ModelBinder(typeof(IdentityDecryptor))] long Id, [FromForm] string RefNo)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_PurchaseOrder", new { Option = "VAL_UN_CHECK", Id, VerifiedBy = _appUser.CommonEmpNo });
            if (errors.Any())
                return UndoError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_PurchaseOrder", new { Option = "UN_CHECK", Id });
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("PURCHASE ORDER", "UnChecked", "PurchaseOrder", Id, RefNo);
            return Message("Purchase order Un-Checked.");
        }

        [ValidateAction(Forms.Procurement.Purchase, Rights.Modify)]
        public async Task<ReturnMessage> ClosePO([ModelBinder(typeof(IdentityDecryptor))] long Id, [FromForm] string RefNo, [FromForm] string ReasonForCancel)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_PurchaseOrder", new { Option = "VAL_PO_CLOSE", Id });
            if (errors.Any())
                return SaveError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_PurchaseOrder", new { Option = "PO_CLOSE", Id, ClosedBy = _appUser.CommonEmpNo, ReasonForCancel });
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("PURCHASE ORDER", ActionType.Edit.ToString(), "PurchaseOrder", Id, RefNo, "Closed");
            return Message("Purchase order marked as Closed.");
        }

        [ValidateAction(Forms.Retail.Quotation, Rights.Modify)]
        public async Task<ReturnMessage> OpenPO([ModelBinder(typeof(IdentityDecryptor))] long Id, [FromForm] string RefNo)
        {
            var errors = await _Webcontext.ValidateAsync("spBSOL_PurchaseOrder", new { Option = "VAL_PO_OPEN", Id });
            if (errors.Any())
                return SaveError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spBSOL_PurchaseOrder", new { Option = "PO_OPEN", Id });
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("PURCHASE ORDER", ActionType.Edit.ToString(), "PurchaseOrder", Id, RefNo, "Open");
            return Message("Purchase order marked as Open.");
        }

        public async Task<List<Supplier>> ReadPOSuppliers()
        {
            return await _Webcontext.Suppliers.Where(x => x.CompanyId == _appUser.CompanyId && x.Active).Select(x => new Supplier { Id = x.Id, SupplierName = x.SupplierName + " - " + x.SupplierCode, GST = x.GST }).ToListAsync();
        }
        public async Task<List<GSTSetting>> ReadGST()
        {
            //return await _Webcontext.PurchaseOrders.Where(x => x.GSTPercent > 0).Select(x => x.GSTPercent).Distinct().ToListAsync();
            return await _Webcontext.GSTSettings.ToListAsync();
        }
        public async Task<List<DropDownModel>> ReadShopGroupNames()
        {
            //return await _Webcontext.PurchaseOrders.Where(x => x.GSTPercent > 0).Select(x => x.GSTPercent).Distinct().ToListAsync();
            return await _Webcontext.ShopGroups.Select(x => new DropDownModel { Id = x.ID, Text = x.Name }).ToListAsync();

        }

        public async Task<DataSourceResult> GetPOItems([DataSourceRequest] DataSourceRequest request, [FromForm] long supplierId = 0)
        {
            request.Sorts.SortInterceptor("SKU");

            var filters = request.Filters.GetFilters();
            var sort = request.Sorts?.FirstOrDefault();

            var result = await _Webcontext.ExecuteSpAsync<PurchaseItemModel>("spBSOL_PurchaseOrderItems", new
            {
                SupplierID = supplierId,
                Page = request.Page - 1,
                request.PageSize,
                SortBy = sort?.Member,
                SorDir = sort?.SortDirection.ToString(),
                SKU = filters.Get("SKUFormatted"),
                UPC = filters.Get("UPC"),
                ItemCode = filters.Get("ItemCode"),
                Description = filters.Get("Description"),
                SupplierCode = filters.Get("SupplierCode"),
                Rate = filters.Get("Rate"),
                Stock = filters.Get("Stock"),
                ItemCategory = filters.Get("ItemCategory")
            });
            return new DataSourceResult { Data = result, Total = result.Select(x => x.TotalCount).FirstOrDefault() };
        }

        public async Task<PurchaseItemModel> SearchPOItems([FromForm] string? searchText = null, [FromForm] long supplierId = 0, [FromForm] long? itemId = null)
        {
            return await _Webcontext.ExecuteSingleAsync<PurchaseItemModel>("spBSOL_PurchaseOrderSearchItem", new { supplierId, searchText, itemId });
        }
        public async Task<List<DropDownModel>> SearchItems([FromForm] string searchText)
        {
            var result = await (from i in _Webcontext.Items
                                where i.Active && (i.SKU.ToString().StartsWith(searchText) || i.UPC.ToString().StartsWith(searchText) ||
                                      i.ItemCode.Contains(searchText) || i.Description.Contains(searchText))
                                      && i.VerifiedOn.HasValue
                                select new DropDownModel
                                {
                                    Id = i.Id,
                                    Text = i.ItemCode + " - " + i.Description
                                }).ToListAsync();
            return result;
        }
        [ValidateAction(Forms.Procurement.Purchase, Rights.View)]
        public async Task<object> GetSupplierIncoTerm([FromForm] long SupplierId)
        {
            return await (from po in _Webcontext.PurchaseOrders
                          where po.SupplierId == SupplierId
                          select new
                          {
                              po.Id,
                              po.PaymentType,
                              po.IncoTerm
                          }).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        }
        public async Task<SupplierPurchaseOrderModel> GetPaymentRequestDetails([ModelBinder(typeof(IdentityDecryptor))] long id)
        {
            var res = await _Webcontext.ExecuteSingleAsync<SupplierPurchaseOrderModel>("spFMS_PaymentRequest", new { Option = "SELECT_PO_PAYMENT", _appUser.CompanyId, id });
            return res;
        }
        [ValidateAction(Forms.Accounts.PaymentRequest, Rights.Modify)]
        public async Task<ReturnMessage> SavePurchasedPaymentRequest([FromForm] PaymentRequest data, [ModelBinder(typeof(IdentityDecryptor))] long purchaseOrderId)
        {
            List<SqlParameter> sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@Option", "VAL_SAVE"),
                new SqlParameter("@ID", data.ID),
                new SqlParameter("@InvoiceNo", data.InvoiceNo),
                new SqlParameter("@InvoiceDate", data.InvoiceDate?? null),
                new SqlParameter("@ReferenceId", data.ReferenceId),
                new SqlParameter("@ReferenceNo", data.ReferenceNo),
                new SqlParameter("@BusinessUnitId", data.BusinessUnitId),
                new SqlParameter("@SupplierId", data.SupplierId),
                new SqlParameter("@PayeeType", data.PayeeName),
                new SqlParameter("@PaymentCategory", data.Category),
                new SqlParameter("@Currency", data.Currency),
                new SqlParameter("@Amount", data.Amount),
                new SqlParameter("@GST", data.GST),
                new SqlParameter("@TotalAmount", data.TotalAmount),
                new SqlParameter("@GSTPercent", data.GSTPercent),
                new SqlParameter("@PaymentMode", data.PaymentType),
                new SqlParameter("@BankId", data.BankId),
                new SqlParameter("@BankName", data.BankName),
                new SqlParameter("@AccountName", data.AccountName),
                new SqlParameter("@AccountNo", data.AccountNo),
                new SqlParameter("@PayeeName", data.PayeeName),
                new SqlParameter("@DebitTo", data.DebitTo),
                new SqlParameter("@CreditTo", data.CreditTo),
                new SqlParameter("@EntryBy", _appUser.CommonEmpNo),
                new SqlParameter("@Remarks", data.Remarks),
                new SqlParameter("@CompanyId", _appUser.CompanyId),
                new SqlParameter("@ParentId", data.ParentId),
                new SqlParameter("@EmployeeId", _appUser.EmployeeId),
                new SqlParameter("@PurchaseOrderId", purchaseOrderId),
                new SqlParameter("@EntityID", _appUser.EntityID)
            };

            var errors = await _Webcontext.ValidateSqlAsync("spFMS_PaymentRequest", sqlParams);
            if (errors.Any())
                return SaveError(errors);
            sqlParams[0].Value = "INSERT_PURCHASE_PAYMENT_REQUEST";

            long Id = await _Webcontext.ExecuteSqlNonQueryWithReturnAsync<long>("spFMS_PaymentRequest", sqlParams);
            if (Id <= 0)
                return Error(Constants.DBErrorMessage);

            await EventLog("PAYMENT REQUEST", ActionType.Add.ToString(), "PaymentRequest", data.ID, data.InvoiceNo);

            return Message("Payment Request saved.", Id);
        }
        #endregion GetPaymentRequestDetails

        #region SupplierInvoice
        [ValidateAction(Forms.Procurement.Purchase, Rights.View)]
        public async Task<DataSourceResult> ReadSupplierInvoices([DataSourceRequest] DataSourceRequest request)
        {
            List<long> shopIds = (from s in _Webcontext.Shops
                                  join se in _Webcontext.ShopEmployees on s.Id equals se.ShopId
                                  where se.EmployeeId == _appUser.EmployeeId && s.Active
                                  select s.Id).ToList();

            var result = (from SI in _Webcontext.SupplierInvoices
                          orderby SI.RefNo
                          join S in _Webcontext.Suppliers on SI.SupplierId equals S.Id
                          join SH in _Webcontext.Shops on SI.ShopId equals SH.Id
                          join SS in _Webcontext.Shipments on SI.ShipmentId equals SS.Id into Shipments
                          from SV in Shipments.DefaultIfEmpty()
                          join A in _Webcontext.Accounts on SI.DebitTo equals A.Id into Accts
                          from acc in Accts.DefaultIfEmpty()
                          let PORefNos = (from SD in _Webcontext.SupplierInvoiceDetails
                                          join PO in _Webcontext.PurchaseOrders on SD.PurchaseOrderId equals PO.Id
                                          where SD.SupplierInvoiceId == SI.Id
                                          select PO.RefNoFormatted).ToList()
                          where SI.Active && S.Active
                          && (_appUser.IsPowerUser || shopIds.Contains(SI.ShopId))
                          select new SupplierInvoice
                          {
                              Id = SI.Id,
                              RefNo = SI.RefNo,
                              SupplierId = SI.SupplierId,
                              ShipmentId = SI.ShipmentId,
                              ShipmentRefNo = SV.RefNoFormatted,
                              ExpenseCategory = SI.ExpenseCategory,
                              Currency = SI.Currency,
                              Description = SI.Description,
                              InvoiceNo = SI.InvoiceNo,
                              InvoiceDate = SI.InvoiceDate,
                              Amount = SI.Amount,
                              GSTSettingId = SI.GSTSettingId,
                              GSTPercent = SI.GSTPercent,
                              GSTAmount = SI.GSTAmount ?? 0,
                              GSTAccountId = SI.GSTAccountId,
                              Balance = SI.Balance,
                              CreatedOn = SI.CreatedOn,
                              SupplierName = S.SupplierName,
                              ShopId = SI.ShopId,
                              ShopName = SH.ShopName + "-" + SH.ShopCode,
                              Debit = acc.AccountName,
                              DebitTo = SI.DebitTo,
                              CreditTo = SI.CreditTo,
                              POSelected = SI.POSelected,
                              GroupReferenceNo = SI.GroupReferenceNo,
                              PORefNos = string.Join(",", PORefNos)
                          });

            return await result.ToDataSourceResultAsync(request);
        }

        [ValidateAction(Forms.Procurement.Purchase, Rights.View)]
        public async Task<DataSourceResult> ReadSupplierInvoiceDetails([DataSourceRequest] DataSourceRequest request, [FromForm] long supplierId = 0, [FromForm] long supplierInvoiceId = 0, [FromForm] string currency = "")
        {
            var result = (from sid in _Webcontext.SupplierInvoiceDetails
                          join po in _Webcontext.PurchaseOrders on sid.PurchaseOrderId equals po.Id
                          join s in _Webcontext.Suppliers on po.SupplierId equals s.Id
                          where po.SupplierId == supplierId && sid.SupplierInvoiceId == supplierInvoiceId && po.Active && po.Currency == currency && s.Active
                          select new SupplierInvoiceDetail
                          {
                              SupplierInvoiceId = sid.SupplierInvoiceId,
                              PurchaseOrderId = sid.PurchaseOrderId,
                              PORefNo = po.RefNoFormatted,
                              Balance = po.Balance + sid.InvoiceAmount,
                              TotalAmount = po.TotalAmount,
                              InvoiceAmount = sid.InvoiceAmount
                          });
            return await result.ToDataSourceResultAsync(request);
        }

        [ValidateAction(Forms.Procurement.Purchase, Rights.View)]
        public async Task<DataSourceResult> ReadWOPSupplierInvoices([DataSourceRequest] DataSourceRequest request, [FromForm] long id = 0, [FromForm] long groupReferenceNo = 0)
        {
            List<SupplierInvoice> result = new List<SupplierInvoice>();
            if (id > 0)
            {
                result = await (from SI in _Webcontext.SupplierInvoices
                                join A in _Webcontext.Accounts on SI.DebitTo equals A.Id into grpA
                                from ACC in grpA.DefaultIfEmpty()
                                join G in _Webcontext.GSTSettings on SI.GSTSettingId equals G.Id into grpG
                                from GG in grpG.DefaultIfEmpty()
                                where SI.GroupReferenceNo == groupReferenceNo && SI.Active
                                select new SupplierInvoice
                                {
                                    Id = SI.Id,
                                    RefNo = SI.RefNo,
                                    InvoiceNo = SI.InvoiceNo,
                                    Amount = SI.Amount,
                                    DebitTo = SI.DebitTo,
                                    Debit = ACC.AccountName,
                                    GSTSettingId = SI.GSTSettingId,
                                    GSTPercent = SI.GSTPercent,
                                    GSTAccountId = SI.GSTAccountId,
                                    GSTAmount = SI.GSTAmount,
                                    GST = GG.Name ?? "",
                                    Description = SI.Description,
                                    GroupReferenceNo = SI.GroupReferenceNo
                                }).ToListAsync();
                return await result.ToDataSourceResultAsync(request);
            }
            for (int index = 0; index < 6; index++)
            {
                SupplierInvoice supplierInvoice = new SupplierInvoice();
                supplierInvoice.Id = 0;
                supplierInvoice.InvoiceNo = "";
                supplierInvoice.Amount = 0;
                supplierInvoice.DebitTo = 0;
                supplierInvoice.Debit = "";
                supplierInvoice.GSTAccountId = 0;
                supplierInvoice.GSTAmount = 0;
                supplierInvoice.GST = "";
                supplierInvoice.Description = "";
                result.Add(supplierInvoice);
            }
            return await result.ToDataSourceResultAsync(request);
        }
        public async Task<StringTypeModel> ReadInvoiceNo()
        {
            var result = await _Webcontext.ExecuteSpAsync<StringTypeModel>("spProcurements_SupplierInvoices_Validation", new { Option = "GET_INVOICENO" });
            return result.FirstOrDefault();
        }
        public async Task<List<DropDownModel>> ReadShipments()
        {
            return await (from sh in _Webcontext.Shipments
                          where sh.CompanyId == _appUser.CompanyId && sh.Active && sh.ClearedDate.HasValue
                          select new DropDownModel { Id = sh.Id, Text = sh.RefNoFormatted }).ToListAsync();
        }
        [ValidateAction(Forms.Procurement.Purchase, Rights.View)]
        public async Task<DataSourceResult> ReadPurchaseOrders([DataSourceRequest] DataSourceRequest request, [FromForm] long supplierId = 0, [FromForm] long supplierInvoiceId = 0, [FromForm] string currency = "")
        {
            var result = (from po in _Webcontext.PurchaseOrders
                          join s in _Webcontext.Suppliers on po.SupplierId equals s.Id
                          where po.SupplierId == supplierId && po.Balance > 0 && po.Active && po.Currency == currency
                          && po.VerifiedOn.HasValue && s.Active
                          && !_Webcontext.SupplierInvoiceDetails.Any(x => x.PurchaseOrderId == po.Id && x.SupplierInvoiceId == supplierInvoiceId)
                          select new
                          {
                              po.Id,
                              po.RefNoFormatted,
                              s.SupplierName,
                              po.TotalAmount,
                              po.Balance,
                              po.PurchaseOrderDate,
                              po.CreatedBy,
                              po.CreatedOn,
                              po.CheckedBy,
                              po.CheckedOn,
                              po.VerifiedBy,
                              po.VerifiedOn,
                              po.ClosedBy,
                              po.ClosedOn,
                              po.Remarks,
                              po.ContactName,
                              po.ContactNo
                          });
            return await result.ToDataSourceResultAsync(request);
        }

        [ValidateAction(Forms.Procurement.Purchase, Rights.Modify)]
        public async Task<ReturnMessage> SaveSupplierInvoice([FromForm] List<SupplierInvoice> supplierInvoices, [FromForm] List<SupplierInvoiceDetail> supplierInvoiceDetails, [FromForm] string refNo = "")
        {
            var supplierTableSchema = new List<SqlMetaData>() {
                    new SqlMetaData("Id", SqlDbType.BigInt),
                    new SqlMetaData("ShopId", SqlDbType.BigInt),
                    new SqlMetaData("SupplierId", SqlDbType.BigInt),
                    new SqlMetaData("ShipmentId", SqlDbType.BigInt),
                    new SqlMetaData("Currency", SqlDbType.VarChar,20),
                    new SqlMetaData("Description", SqlDbType.VarChar,-1),
                    new SqlMetaData("InvoiceNo", SqlDbType.VarChar,50),
                    new SqlMetaData("InvoiceDate", SqlDbType.Date),
                    new SqlMetaData("Amount", SqlDbType.Money),
                    new SqlMetaData("DebitTo", SqlDbType.BigInt),
                    new SqlMetaData("CreditTo", SqlDbType.BigInt),
                    new SqlMetaData("GSTSettingId", SqlDbType.BigInt),
                    new SqlMetaData("GSTPercent", SqlDbType.Money),
                    new SqlMetaData("GSTAmount", SqlDbType.Money),
                    new SqlMetaData("GSTAccountId", SqlDbType.BigInt),
                    new SqlMetaData("CreatedBy", SqlDbType.VarChar,150),
                    new SqlMetaData("ExpenseCategory",SqlDbType.VarChar,10),
                    new SqlMetaData("POSelected",SqlDbType.Bit),
                    new SqlMetaData("GroupReferenceNo", SqlDbType.BigInt)
                }.ToArray();

            var sqlInvoiceDataRecords = new List<SqlDataRecord>();
            supplierInvoices.ForEach(x =>
            {
                SqlDataRecord sqlDataRecord = new SqlDataRecord(supplierTableSchema);
                sqlDataRecord.SetInt64(0, x.Id);
                sqlDataRecord.SetInt64(1, x.ShopId);
                sqlDataRecord.SetInt64(2, x.SupplierId);
                sqlDataRecord.SetSqlInt64(3, x.ShipmentId.HasValue ? x.ShipmentId.Value : SqlInt64.Null);
                sqlDataRecord.SetString(4, x.Currency);
                sqlDataRecord.SetSqlString(5, !string.IsNullOrEmpty(x.Description) ? x.Description : SqlString.Null);
                sqlDataRecord.SetSqlString(6, !string.IsNullOrEmpty(x.InvoiceNo) ? x.InvoiceNo : SqlString.Null);
                sqlDataRecord.SetDateTime(7, x.InvoiceDate);
                sqlDataRecord.SetDecimal(8, x.Amount);
                sqlDataRecord.SetInt64(9, x.DebitTo.Value);
                sqlDataRecord.SetInt64(10, x.CreditTo.Value);
                sqlDataRecord.SetSqlInt64(11, x.GSTSettingId.HasValue ? x.GSTSettingId.Value : SqlInt64.Null);
                sqlDataRecord.SetDecimal(12, x.GSTPercent ?? 0);
                sqlDataRecord.SetDecimal(13, x.GSTAmount ?? 0);
                sqlDataRecord.SetSqlInt64(14, x.GSTAccountId.HasValue ? x.GSTAccountId.Value : SqlInt64.Null);
                sqlDataRecord.SetString(15, _appUser.CommonEmpNo);
                sqlDataRecord.SetSqlString(16, !string.IsNullOrEmpty(x.ExpenseCategory) ? x.ExpenseCategory : SqlString.Null);
                sqlDataRecord.SetBoolean(17, x.POSelected);
                sqlDataRecord.SetSqlInt64(18, supplierInvoices[0].GroupReferenceNo.HasValue ? supplierInvoices[0].GroupReferenceNo.Value : SqlInt64.Null);
                sqlInvoiceDataRecords.Add(sqlDataRecord);
            });

            var tableSchema = new List<SqlMetaData>() {
                    new SqlMetaData("PurchaseOrderId", SqlDbType.BigInt),
                    new SqlMetaData("InvoiceAmount", SqlDbType.Money)
                }.ToArray();

            var sqlDataRecords = new List<SqlDataRecord>();
            if (supplierInvoiceDetails != null && supplierInvoiceDetails.Any())
            {
                supplierInvoiceDetails.ForEach(x =>
                {
                    SqlDataRecord sqlDataRecord = new SqlDataRecord(tableSchema);
                    sqlDataRecord.SetInt64(0, x.PurchaseOrderId);
                    sqlDataRecord.SetDecimal(1, x.InvoiceAmount);
                    sqlDataRecords.Add(sqlDataRecord);
                });
            }

            List<SqlParameter> sqlInvoiceParams = new List<SqlParameter>
                {
                    new SqlParameter("@Option","VAL_SAVE"),
                    new SqlParameter("@Id",supplierInvoices[0].Id),
                    new SqlParameter("@CreatedBy",_appUser.CommonEmpNo),
                    new SqlParameter
                    {
                        ParameterName = "@SupplierInvoiceType",
                        Value = sqlInvoiceDataRecords,
                        SqlDbType = SqlDbType.Structured,
                        TypeName = "dbo.SupplierInvoiceType"
                    }
            };
            if (supplierInvoiceDetails != null && supplierInvoiceDetails.Any())
            {
                SqlParameter parameter = new SqlParameter
                {
                    ParameterName = "@SupplierInvoiceDetailType",
                    Value = sqlDataRecords,
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.SupplierInvoiceDetailType"
                };
                sqlInvoiceParams.Add(parameter);
            }

            var errors = await _Webcontext.ValidateSqlAsync("spProcurements_SupplierInvoices_Validation", sqlInvoiceParams);
            if (errors.Any())
                return SaveError(errors);

            sqlInvoiceParams[0].Value = supplierInvoices[0].Id > 0 ? "UPDATE" : "INSERT";
            var retVal = await _Webcontext.ExecuteSqlNonQueryAsync("spProcurements_SupplierInvoices", sqlInvoiceParams);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            if (supplierInvoices[0].Id > 0)
                await EventLog("SUPPLIER INVOICE", ActionType.Edit.ToString(), "SupplierInvoices", supplierInvoices[0].Id, refNo);
            return Message("Supplier invoice details saved.");
        }

        [ValidateAction(Forms.Procurement.Purchase, Rights.Delete)]
        public async Task<ReturnMessage> DeleteSupplierInvoice([FromForm] SupplierInvoice data)
        {
            var errors = await _Webcontext.ValidateAsync("spProcurements_SupplierInvoices_Validation", new { Option = "VAL_DELETE", data.Id });
            if (errors.Any())
                return DeleteError(errors);

            int retVal = await _Webcontext.ExecuteNonQueryAsync("spProcurements_SupplierInvoices", new { Option = "DELETE", data.Id });
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            await new Document() { DocumentRoot = _commonHelper.GetDocumentRoot() }.DeleteByReference(DocumentReference.SupplierInvoices, data.Id.ToString());

            await EventLog("SUPPLIER INVOICE", ActionType.Delete.ToString(), "SupplierInvoices", data.Id, data.RefNo);
            return Message("Supplier invoice deleted.");
        }

        #endregion

        #endregion

        #region ShopEmployees

        [ValidateAction(Forms.Procurement.Shops, Rights.View)]
        public async Task<DataSourceResult> ReadEmployees([DataSourceRequest] DataSourceRequest request, [FromForm] long? ShopId = 0)
        {
            var result = await _Webcontext.ExecuteSpAsync<EmployeeDetail>("spProcurements_ShopEmployees", new { Option = "GET_EMPLOYEES", ShopId });
            return result.ToDataSourceResult(request);
        }

        [ValidateAction(Forms.Procurement.Shops, Rights.View)]
        public async Task<DataSourceResult> ReadShopEmployees([DataSourceRequest] DataSourceRequest request, [FromForm] long? ShopId)
        {
            var result = await _Webcontext.ExecuteSpAsync<ShopEmployee>("spProcurements_ShopEmployees", new { Option = "SELECT", ShopId });
            return result.ToDataSourceResult(request);

            //await _context.ShopEmployees.ToListAsync();
        }
        [ValidateAction(Forms.Procurement.Shops, Rights.Modify)]
        public async Task<ReturnMessage> SaveShopEmployees([FromForm] string shopEmployees, [FromForm] long shopId)
        {
            var tableSchema = new List<SqlMetaData>() {
                new SqlMetaData("Number", SqlDbType.BigInt)
            }.ToArray();
            var sqlDataRecords = new List<SqlDataRecord>();
            shopEmployees.Split(',').ToList().ForEach(x =>
            {
                SqlDataRecord sqlDataRecord = new SqlDataRecord(tableSchema);
                sqlDataRecord.SetInt64(0, long.Parse(x));
                sqlDataRecords.Add(sqlDataRecord);
            });
            List<SqlParameter> sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@Option","INSERT"),
                new SqlParameter("@ShopId",shopId)
            };

            SqlParameter parameter = new SqlParameter
            {
                ParameterName = "@Employees",
                Value = sqlDataRecords,
                SqlDbType = SqlDbType.Structured,
                TypeName = "dbo.NumberType"
            };

            sqlParams.Add(parameter);
            await _Webcontext.Database.ExecuteSqlRawAsync("EXEC spProcurements_ShopEmployees @Option=@Option, @ShopId = @ShopId,@Employees=@Employees", sqlParams);
            return Message("Shop employees saved.");
        }

        [ValidateAction(Forms.Procurement.Shops, Rights.Delete)]
        public async Task<ReturnMessage> DeleteShopEmployee([FromForm] ShopEmployee data)
        {
            int retVal = await _context.ExecuteNonQueryAsync("spProcurements_ShopEmployees", new { Option = "DELETE_SHOPEMPLOYEE", data.ShopId, data.EmployeeId });
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);
            //if (!await data.RemoveAsync())
            //    return DeleteError(data.ErrorMessage);
            await EventLog("Shop Employees", ActionType.Delete.ToString(), "ShopEmployees", data.ShopId, data.ShopName + " - " + data.CommonEmpNo);
            return Message("Shop employee deleted.");
        }

        #endregion

        #region Purchase Return      
        public async Task<DataSourceResult> ReadPurchaseReturns([DataSourceRequest] DataSourceRequest request, [FromForm] int? sku = 0)
        {
            var result = await _Webcontext.ExecuteSpAsync<PurchaseReturn>("spBSOL_PurchaseReturn", new { Option = "SELECT", _appUser.IsPowerUser, _appUser.ShopId, sku });
            return result.ToDataSourceResult(request);
        }

        [ValidateAction(Forms.Procurement.PurchaseReturn, Rights.View)]
        public async Task<DataSourceResult> ReadItemReturn([DataSourceRequest] DataSourceRequest request, [FromForm] long purchaseReturnId, [FromForm] long supplierInvoiceId, [FromForm] bool selected, [FromForm] long shipmentId = 0)
        {
            List<PurchaseOrderDetail> result = null;
            if (supplierInvoiceId > 0)
            {
                if (shipmentId > 0)
                {
                    result = (from sp in _Webcontext.ShipmentPurchaseOrderDetails
                              join pod in _Webcontext.PurchaseOrderDetails on sp.PurchaseOrderDetailId equals pod.Id
                              join im in _Webcontext.Items on pod.ItemId equals im.Id
                              where sp.ShipmentId == shipmentId
                              select new PurchaseOrderDetail
                              {
                                  Id = pod.Id,
                                  PurchaseOrderId = pod.PurchaseOrderId,
                                  SKU = im.SKU,
                                  UPC = im.UPC,
                                  ItemCode = im.ItemCode,
                                  ItemDescription = im.Description,
                                  ItemId = im.Id,
                                  ConfirmedQty = sp.ConfirmedQty,
                                  Rate = pod.Rate,
                                  ReturnedQty = sp.ReceivedOn.HasValue ? sp.ReceivedQty : sp.ConfirmedQty,
                                  TotalPrice = Convert.ToDecimal(sp.ConfirmedQty) * pod.Rate,
                                  ImagePath = im.ImagePath,
                                  PurchaseUnit = pod.PurchaseUnit,
                                  Size = im.Size,
                                  Unit = im.Unit
                              }).ToList();
                }
                else
                {
                    result = (from si in _Webcontext.SupplierInvoices
                              join sid in _Webcontext.SupplierInvoiceDetails on si.Id equals sid.SupplierInvoiceId
                              join pod in _Webcontext.PurchaseOrderDetails on sid.PurchaseOrderId equals pod.PurchaseOrderId
                              join po in _Webcontext.PurchaseOrders on pod.PurchaseOrderId equals po.Id
                              join im in _Webcontext.Items on pod.ItemId equals im.Id
                              join ic in _Webcontext.ItemCategories on im.ItemCategoryId equals ic.Id
                              where si.Id == supplierInvoiceId && si.Active
                              select new PurchaseOrderDetail
                              {
                                  Id = pod.Id,
                                  PurchaseOrderId = pod.PurchaseOrderId,
                                  SKU = im.SKU,
                                  UPC = im.UPC,
                                  ItemCode = im.ItemCode,
                                  ItemDescription = im.Description,
                                  ItemId = im.Id,
                                  ConfirmedQty = pod.ActualConfirmedQty - pod.ActualReturnedQty,
                                  Rate = pod.Rate,
                                  ReturnedQty = pod.ActualReturnedQty == 0 ? pod.ActualConfirmedQty : pod.ActualReturnedQty,
                                  TotalPrice = 0,
                                  Selected = selected && pod.ActualReturnedQty != 0,
                                  ImagePath = im.ImagePath,
                                  PurchaseUnit = pod.PurchaseUnit,
                                  Size = im.Size,
                                  Unit = im.Unit
                              }).ToList();
                    result = result.Where(x => x.ConfirmedQty > 0).ToList();
                }
            }
            else if (purchaseReturnId > 0)
            {
                result = (from pr in _Webcontext.PurchaseReturnDetails
                          join pod in _Webcontext.PurchaseOrderDetails on pr.PurchaseOrderDetailId equals pod.Id
                          join im in _Webcontext.Items on pod.ItemId equals im.Id
                          where pr.PurchaseReturnMasterId == purchaseReturnId
                          select new PurchaseOrderDetail
                          {
                              Id = pr.PurchaseOrderDetailId,
                              SKU = im.SKU,
                              UPC = im.UPC,
                              ItemCode = im.ItemCode,
                              ItemDescription = im.Description,
                              ItemId = im.Id,
                              ConfirmedQty = (decimal)(shipmentId > 0 ? Convert.ToDecimal(pr.ActualReturnedQty) : Convert.ToDecimal(pod.ActualConfirmedQty)),
                              Rate = pod.Rate,
                              ReturnedQty = (decimal)pr.ActualReturnedQty,
                              TotalPrice = pod.Rate * Convert.ToDecimal(pr.ActualReturnedQty),
                              Selected = selected && pr.ActualReturnedQty != 0,
                              ImagePath = im.ImagePath,
                              PurchaseUnit = pod.PurchaseUnit,
                              Size = im.Size,
                              Unit = im.Unit
                          }).ToList();
            }
            return await result.ToDataSourceResultAsync(request);
        }

        [ValidateAction(Forms.Procurement.PurchaseReturn, Rights.Modify)]
        public async Task<ReturnMessage> SavePurchaseReturn(PurchaseReturnSaveModel data)
        {
            var tableSchema = new List<SqlMetaData>() {
                new SqlMetaData("ItemId", SqlDbType.BigInt),
                new SqlMetaData("ReturnedQty", SqlDbType.Decimal,18,2),
                new SqlMetaData("PurchaseOrderDetailId", SqlDbType.BigInt),
            }.ToArray();

            var sqlDataRecords = new List<SqlDataRecord>();
            data.ReturnItemDetails.ForEach(x =>
            {
                SqlDataRecord sqlDataRecord = new SqlDataRecord(tableSchema);
                sqlDataRecord.SetInt64(0, x.ItemId);
                sqlDataRecord.SetDecimal(1, (decimal)x.ReturnedQty);
                sqlDataRecord.SetInt64(2, x.PurchaseOrderDetailId);
                sqlDataRecords.Add(sqlDataRecord);
            });

            List<SqlParameter> sqlValidationParams = new List<SqlParameter>
            {
                new SqlParameter("@Option", "VAL_SAVE"),
                new SqlParameter("@PurchaseReturnId", data.Id),
                new SqlParameter("@ShopId", _appUser.ShopId),
                new SqlParameter("@SupplierInvoiceId", data.SupplierInvoiceId),
                new SqlParameter("@ReturnedDate", data.ReturnedDate),
                new SqlParameter("@ShipmentId", data.ShipmentId),
                new SqlParameter
                {
                    ParameterName = "@PurchaseReturnType",
                    Value = sqlDataRecords,
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.PurchaseReturnType"
                }
            };
            var errors = await _Webcontext.ValidateSqlAsync("spBSOL_PurchaseReturn", sqlValidationParams);
            if (errors.Any())
                return SaveError(errors);

            List<SqlParameter> sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@Option", data.Id > 0 ? "UPDATE" : "INSERT"),
                new SqlParameter("@PurchaseReturnId", data.Id),
                new SqlParameter("@SupplierInvoiceId", data.SupplierInvoiceId),
                new SqlParameter("@ReturnedDate", data.ReturnedDate),
                new SqlParameter("@Remarks", data.Remarks),
                new SqlParameter("@TotalAmount", data.TotalAmount),
                new SqlParameter("@DiscountAmount", data.DiscountAmount),
                new SqlParameter("@CreatedBy", _appUser.CommonEmpNo),
                new SqlParameter("@PartialReturn", data.PartialReturn),
                new SqlParameter("@ShipmentId", data.ShipmentId),
                new SqlParameter
                {
                    ParameterName = "@PurchaseReturnType",
                    Value = sqlDataRecords,
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.PurchaseReturnType"
                }
            };

            long id = await _Webcontext.ExecuteSqlNonQueryWithReturnAsync<long>("spBSOL_PurchaseReturn", sqlParams);
            if (id <= 0)
                return Error(Constants.DBErrorMessage);

            sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@Option", "UPDATE_RETURN_ITEMS"),
                new SqlParameter("@SupplierInvoiceId", data.SupplierInvoiceId),
                new SqlParameter("@PurchaseReturnId", id),
                new SqlParameter("@CreatedBy", _appUser.CommonEmpNo),
                new SqlParameter("@ReturnedDate", data.ReturnedDate),
                new SqlParameter("@ShipmentId", data.ShipmentId),
                new SqlParameter
                {
                    ParameterName = "@PurchaseReturnType",
                    Value = sqlDataRecords,
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.PurchaseReturnType"
                }
            };
            int retVal = await _Webcontext.ExecuteSqlNonQueryAsync("spBSOL_PurchaseReturn", sqlParams);
            if (retVal <= 0)
                return Error(Constants.DBErrorMessage);

            if (data.Id > 0)
                await EventLog("Purchase Return", ActionType.Edit.ToString(), "PurchaseReturn", data.Id, data.RefNoFormatted);
            return Message("Purchase return details saved.", id);
        }

        [ValidateAction(Forms.Procurement.PurchaseReturn, Rights.Delete)]
        public async Task<object> DeletePurchaseReturn([FromForm] PurchaseReturn data)
        {
            int retValue = await _Webcontext.ExecuteNonQueryAsync("spBSOL_PurchaseReturn", new { Option = "DELETE", PurchaseReturnId = data.Id, data.SupplierInvoiceId, data.ShipmentId });
            if (retValue <= 0)
                return Error(Constants.DBErrorMessage);

            await new Document() { DocumentRoot = _commonHelper.GetDocumentRoot() }.DeleteByReference(DocumentReference.PurchaseReturns, data.Id.ToString());

            await EventLog("Purchase Return", ActionType.Delete.ToString(), "PurchaseReturn", data.Id, data.RefNoFormatted);
            return Message("Purchase return details deleted.");
        }
        public async Task<DataTable> PrintPurchaseReturn(long id)
        {
            return await _context.ExecuteDataTableAsync("spBSOL_ReportPurchaseReturnMini", new { id });
        }
        #endregion
        #region CurrencyHSNSettings
        [ValidateAction(Forms.Procurement.ControlPanel, Rights.View)]
        public async Task<DataSourceResult> ReadCurrencies([DataSourceRequest] DataSourceRequest request)
        {
            var result = _Webcontext.Currencies.ToList();
            return await result.ToDataSourceResultAsync(request);
        }
        [ValidateAction(Forms.Procurement.ControlPanel, Rights.Modify)]
        public async Task<DataSourceResult> UpdateCurrencies([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "Models")] IEnumerable<Curency> datas)
        {
            foreach (var currencies in datas)
            {
                if (!await currencies.SaveAsync())
                    ModelState.AddModelError(currencies.ErrorMessage);
            }
            return datas.ToDataSourceResult(request, ModelState);
        }
        [ValidateAction(Forms.Procurement.ControlPanel, Rights.Delete)]
        public async Task<ReturnMessage> DeleteCurrency([FromForm] Curency data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Currency setting deleted successfully.");
        }
        [ValidateAction(Forms.Procurement.ControlPanel, Rights.View)]
        public async Task<DataSourceResult> ReadHSNSettings([DataSourceRequest] DataSourceRequest request)
        {
            var result = _Webcontext.HSNSettings.ToList();
            return await result.ToDataSourceResultAsync(request);
        }
        [ValidateAction(Forms.Procurement.ControlPanel, Rights.Modify)]
        public async Task<DataSourceResult> UpdateHSNSettings([DataSourceRequest] DataSourceRequest request, [FromForm][Bind(Prefix = "Models")] IEnumerable<HSNSetting> datas)
        {
            foreach (var hsnSetting in datas)
            {
                if (!await hsnSetting.SaveAsync())
                    ModelState.AddModelError(hsnSetting.ErrorMessage);
            }
            return datas.ToDataSourceResult(request, ModelState);
        }
        [ValidateAction(Forms.Procurement.ControlPanel, Rights.Delete)]
        public async Task<ReturnMessage> DeleteHSNSetting([FromForm] HSNSetting data)
        {
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("HSN setting deleted successfully.");
        }
        #endregion
    }
}