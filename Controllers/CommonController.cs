using BSOL.Core;
using BSOL.Core.Entities;
using BSOL.Core.Models.Common;
using BSOL.Helpers;
using BSOL.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Net.Mime;
using static SQLite.SQLite3;

namespace BSOL.Controllers
{
    public class CommonController : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICommonHelper _commonHelper;
        private readonly IConfiguration _configuration;
        public CommonController(BSOLContext context, BSOLWebContext Webcontext, AppUser appUser, IWebHostEnvironment hostingEnvironment, ICommonHelper commonHelper, IConfiguration configuration) : base(context, Webcontext, appUser)
        {
            _hostingEnvironment = hostingEnvironment;
            _commonHelper = commonHelper;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<List<string>> ReadCountries()
        {
            return await _Webcontext.Countries.Select(x => x.CountryName).ToListAsync();
        }
        [HttpPost]
        public async Task<List<DropDownModel>> ReadAtoll()
        {
            return await _Webcontext.Atolls.Select(x => new DropDownModel { Id = x.ID, Text = x.AtollName }).Distinct().ToListAsync();
        }
        [HttpPost]
        public async Task<List<DropDownModel>> ReadIsland([FromForm] long AtollId)
        {
            return await _Webcontext.Islands.Where(x => x.AtollID == AtollId).Select(x => new DropDownModel { Id = x.ID, Text = x.IslandName }).Distinct().ToListAsync();
        }
        [HttpPost]
        public async Task<List<Island>> ReadIslandName([FromForm] long AtollId)
        {
            return await _Webcontext.Islands.Where(x => x.AtollID == AtollId).Select(x => new Island { IslandName = x.IslandName, IslandCode = x.IslandCode, ID = x.ID }).Distinct().ToListAsync();
        }
        [HttpPost]
        public async Task<List<string>> ReadStates([FromForm] string Country)//Island
        {
            return await _Webcontext.States.Where(x => x.Country.CountryName == Country).Select(x => x.Name).ToListAsync();
        }

        [HttpPost]
        public async Task<List<string>> ReadCities([FromForm] string State)//Atoll
        {
            return await _Webcontext.Cities.Where(x => x.State.Name == State).Select(x => x.Name).ToListAsync();
        }

        [HttpPost]
        public async Task<List<DropDownModel>> ReadAccountsByType([FromQuery] string type)
        {
            return await (from a in _Webcontext.Accounts
                          join dt in _Webcontext.AccountDetailTypes on a.AccountDetailTypeId equals dt.Id
                          join at in _Webcontext.AccountTypes on dt.AccountTypeId equals at.Id
                          where at.CompanyId == _appUser.CompanyId && (string.IsNullOrEmpty(type) || at.Category == type)
                          select new DropDownModel
                          {
                              Id = a.Id,
                              Text = string.Concat(a.AccountName, " - ", a.Code)
                          }).ToListAsync();
        }

        [HttpPost]
        public async Task<List<Curency>> ReadCurrencies()
        {
            return await _Webcontext.Currencies.Select(x => new Curency { Currency = x.Currency, ConversionRate = x.ConversionRate }).ToListAsync();
        }
        [HttpPost]
        public async Task<List<StringTypeModel>> ReadSelectedCurrencies()
        {
            return await _Webcontext.Currencies.Where(x => x.Currency == "MVR" || x.Currency == "USD").Select(x => new StringTypeModel { Value = x.Currency }).ToListAsync();
        }

        [HttpPost]
        public async Task<List<DropDownModel>> ReadAccountTypes()
        {
            return await _Webcontext.AccountTypes.Where(x => x.CompanyId == _appUser.CompanyId).Select(x => new DropDownModel { Id = x.Id, Text = x.Type }).ToListAsync();
        }

        [HttpPost]
        public async Task<List<DropDownModel>> ReadShopGroup()
        {
            return await _Webcontext.ShopGroups.Select(x => new DropDownModel { Id = x.ID, Text = x.Name }).ToListAsync();
        }
        [HttpPost]
        public async Task<List<DropDownTreeModel>> ReadAccountDetailTypes([FromForm] long accountTypeId)
        {
            List<DropDownTreeModel> result = await _Webcontext.AccountDetailTypes.Where(x => x.AccountTypeId == accountTypeId).Select(x => new DropDownTreeModel
            {
                Id = x.Id,
                Text = x.DetailType,
                ParentId = x.ParentId
            }).ToListAsync();
            result.ForEach(item => item.Items = result.Where(child => child.ParentId == item.Id).ToList());
            return result.Where(item => item.ParentId == null).ToList();
        }

        [HttpPost]
        public async Task<List<DropDownModel>> ReadSuppliers([FromForm] string payeeType = null)
        {
            if (payeeType == "Supplier")
                return await _Webcontext.Suppliers.Where(x => x.CompanyId == _appUser.CompanyId && x.Active).Select(x => new DropDownModel { Id = x.Id, Text = x.SupplierName + " - " + x.SupplierCode }).ToListAsync();

            else if (payeeType == "Staff")
                return await _Webcontext.Employees.Where(x => x.EntityID == _appUser.EntityID && x.Active).Select(x => new DropDownModel { Id = x.ID, Text = x.ShortName + " - " + x.EmpID }).ToListAsync();

            else if (payeeType == "Customer")
                return await _Webcontext.Customers.Where(x => x.CompanyId == _appUser.CompanyId && x.Active).Select(x => new DropDownModel { Id = x.ID, Text = x.NameEN }).ToListAsync();
            
            else
                return await _Webcontext.Suppliers.Where(x => x.CompanyId == _appUser.CompanyId && x.Active).Select(x => new DropDownModel { Id = x.Id, Text = x.SupplierName + " - " + x.SupplierCode }).ToListAsync();

        }
        public async Task<List<DropDownModel>> ReadShopGroups()
        {
            return await _Webcontext.ShopGroups.Select(x => new DropDownModel { Id = x.ID, Text = x.Name }).ToListAsync();
        }
        public async Task<List<string>> ReadServiceCategory()
        {
            return await _Webcontext.BusinessServices.Select(x => x.ServiceCategory).Distinct().ToListAsync();
        }

        [HttpPost]
        public async Task<object> ReadBankAccounts()
        {
            return await (from a in _Webcontext.Accounts
                          join adt in _Webcontext.AccountDetailTypes on a.AccountDetailTypeId equals adt.Id
                          join cr in _Webcontext.Currencies on a.Currency equals cr.Currency
                          where adt.DetailType == "Cash In Bank"
                          select new
                          {
                              a.Id,
                              AccountName = string.Concat(a.AccountName, " - ", a.Code),
                              a.Currency,
                              cr.ConversionRate
                          }).ToListAsync();
        }

        [HttpPost]
        public async Task<List<DropDownModel>> ReadFinanceEmployees()
        {
            var result = await (from eg in _Webcontext.Employees
                                join em in _Webcontext.Employments on eg.ID equals em.ID
                                join d in _Webcontext.Designations on em.DesignationID equals d.ID
                                join s in _Webcontext.Sections on d.SectionID equals s.ID
                                join dp in _Webcontext.Departments on s.DepartmentID equals dp.ID
                                where eg.CompanyID == _appUser.CompanyId && eg.Active && dp.DepartmentName == "Accounts"
                                select new DropDownModel { Id = eg.ID, Text = eg.ShortName + " - " + eg.EmpID }).ToListAsync();
            return result;
        }
        [HttpPost]
        public async Task<List<string>> ReadFinanceAndHPEmployees()
        {
            var result = await (from eg in _Webcontext.Employees
                                join em in _Webcontext.Employments on eg.ID equals em.ID
                                join d in _Webcontext.Designations on em.DesignationID equals d.ID
                                join s in _Webcontext.Sections on d.SectionID equals s.ID
                                join dp in _Webcontext.Departments on s.DepartmentID equals dp.ID
                                where eg.CompanyID == _appUser.CompanyId && eg.Active && (dp.DepartmentName == "Accounts" || dp.DepartmentName == "HP Accounts")
                                select eg.ShortName + " - " + eg.EmpID).ToListAsync();
            return result;
        }

        [HttpPost]
        public async Task<List<DropDownModel>> ReadAllEmployees()
        {
            return await _Webcontext.Employees.Where(x => x.CompanyID == _appUser.CompanyId && x.Active).Select(x => new DropDownModel { Id = x.ID, Text = string.Format("{0} - {1}", x.ShortName, x.EmpID) }).ToListAsync();
        }

        [HttpPost]
        public async Task<List<string>> ReadCommonName()
        {
            return await _Webcontext.Employees.Where(x => x.Active).Select(x => x.ShortName + "-" + x.EmpID).Distinct().ToListAsync();
        }
        [HttpPost]
        public async Task<List<string>> ReadAccountsStaff()
        {
            var result = await (from eg in _Webcontext.Employees
                                join em in _Webcontext.Employments on eg.ID equals em.ID
                                join d in _Webcontext.Designations on em.DesignationID equals d.ID
                                join s in _Webcontext.Sections on d.SectionID equals s.ID
                                join dp in _Webcontext.Departments on s.DepartmentID equals dp.ID
                                where eg.CompanyID == _appUser.CompanyId && eg.Active && dp.DepartmentName == "Accounts"
                                select eg.ShortName + " - " + eg.EmpID).ToListAsync();
            return result;
        }
        [HttpPost]
        public async Task<List<string>> ReadHandOverTo([FromForm] string payeeType = null)
        {
            if (payeeType == null || payeeType == "Supplier")
            {
                var result = await (from sp in _Webcontext.Suppliers
                                    where sp.Active
                                    select sp.SupplierName + " - " + sp.SupplierCode).ToListAsync();
                return result;

            }
            else
            {
                var result = await (from eg in _Webcontext.Employees
                                    join em in _Webcontext.Employments on eg.ID equals em.ID
                                    where eg.CompanyID == _appUser.CompanyId && eg.Active && eg.EntityID == _appUser.EntityID
                                    select eg.ShortName + " - " + eg.EmpID).ToListAsync();
                return result;
            }
        }

        [HttpPost]
        public async Task<List<DropDownModel>> ReadRequestedPaymentList()
        {
            return await _Webcontext.PaymentRequestACCS.Select(x => new DropDownModel { Id = x.ID, Text = string.IsNullOrEmpty(x.PageRefNo) ? x.RefNoFormatted : x.PageRefNo }).ToListAsync();
        }
        [HttpPost]
        public async Task<List<DropDownModel>> ReadShops()
        {
            return await _Webcontext.Shops.Where(x => x.Active).Select(x => new DropDownModel { Id = x.Id, Text = x.ShopName }).ToListAsync();
        }
        [HttpPost]
        public async Task<List<DropDownModel>> ReadAllShops()
        {
            return await _Webcontext.Shops.Select(x => new DropDownModel { Id = x.Id, Text = x.ShopName }).ToListAsync();
        }
        [HttpPost]
        public async Task<List<string>> ReadNationality()
        {
            return await _Webcontext.Countries.Where(x => !string.IsNullOrEmpty(x.Nationality)).Select(x => x.Nationality).ToListAsync();
        }
        [HttpPost]
        public async Task<List<DropDownModel>> ReadSearchCustomer([FromForm] string searchText)
        {
            var result = await (from a in _context.CustomerDetails
                                where a.Cancel_Flag == false && (a.Cust_Name_EN.Contains(searchText) || a.Cust_Ref.Contains(searchText))
                                select new DropDownModel
                                {
                                    Id = a.tbl_ID,
                                    Text = a.Cust_Name_EN + " - " + a.Cust_Ref
                                }).ToListAsync();
            return result;
        }
        public async Task<DataSourceResult> ReadPickCustomer([DataSourceRequest] DataSourceRequest request)
        {
            var result = (from eg in _context.CustomerDetails
                          where eg.Cancel_Flag == false
                          select new CustomerDetail
                          {
                              tbl_ID = eg.tbl_ID,
                              Cust_Ref = eg.Cust_Ref,
                              Cust_Name_EN = eg.Cust_Name_EN,
                              Cust_ContactNo = eg.Cust_ContactNo,
                              Cust_PresentAddress_En = eg.Cust_Per_Address_EN
                          });
            return await result.ToDataSourceResultAsync(request);
        }
        [HttpPost]
        public async Task<List<string>> ReadProduct()
        {
            return await _context.ItemCategorys.Where(x => (x.ItemCategoryCode == "OBM" || x.ItemCategoryCode == "MC" || x.ItemCategoryCode == "WV" || x.ItemCategoryCode == "PARTS" || x.ItemCategoryCode == "SPARE PARTS")).Select(x => x.ItemCategoryCode).Distinct().ToListAsync();
        }

        [HttpPost]
        public async Task<List<string>> ReadSalesProduct()
        {
            return await _context.ShipmentSerialNos.Select(x => x.ProductCode).Distinct().ToListAsync();
        }
        public async Task<List<DropDownModel>> ReadItemCategory()
        {
            var result = await _Webcontext.ItemCategories.Select(x => new DropDownModel { Id = x.Id, Text = x.Category }).ToListAsync();
            return result;
        }

        public async Task<List<DropDownModel>> ReadSalesCategory()
        {
            var result = await _Webcontext.SalesCategories.Select(x => new DropDownModel { Id = x.Id, Text = x.Category }).ToListAsync();
            return result;
        }
        public async Task<List<HSNSetting>> ReadHSNSettings()
        {
            return await _Webcontext.HSNSettings.Select(x => new HSNSetting { Id = x.Id, HSNCode = x.HSNCode, IsDefault = x.IsDefault }).ToListAsync();
        }
        public async Task<List<DropDownModel>> ReadOriginName()
        {
            var result = await _Webcontext.Items.Where(x => x.OriginName != null).Select(x => new DropDownModel { Text = x.OriginName }).Distinct().ToListAsync();
            return result;
        }

        public async Task<List<DropDownModel>> ReadBrand()
        {
            var result = await _Webcontext.Items.Where(x => x.Brand != null).Select(x => new DropDownModel { Text = x.Brand }).Distinct().ToListAsync();
            return result;
        }

        public async Task<List<Category>> ReadCategory(long EntityID = 0)
        {
            EntityID = EntityID == 0 ? _appUser.EntityID : EntityID;
            return await _Webcontext.Categories.Where(x => x.EntityID == EntityID).Select(x => new Category { ID = x.ID, CategoryName = x.CategoryName }).ToListAsync();
        }
        public async Task<List<DropDownModel>> ReadVendorName()
        {
            var result = await _Webcontext.Items.Where(x => x.VendorName != null).Select(x => new DropDownModel { Text = x.VendorName }).Distinct().ToListAsync();
            return result;
        }
        public FileContentResult ItemImage([FromQuery] string img)
        {
            bool isItemImg = Convert.ToBoolean(HttpContext.Request.Query["isItem"]);
            if (!img.IsValid())
                return File(System.IO.File.ReadAllBytes(Path.Combine(_hostingEnvironment.WebRootPath, "Images", isItemImg ? "NoItemImage.png" : "NoImageForPO.png")), System.Net.Mime.MediaTypeNames.Image.Jpeg);

            string dirPath = Path.Combine(_commonHelper.GetDocumentRoot(), ImagePath.ItemImage.ToString()); //Path.Combine(_commonHelper.GetConstant(Helpers.Constant.DOCUMENT), _appUser.UniqueID, "ItemImage");
            string imagePath = Path.Combine(dirPath, img);
            if (Directory.Exists(dirPath) && System.IO.File.Exists(imagePath))
                return File(System.IO.File.ReadAllBytes(imagePath), System.Net.Mime.MediaTypeNames.Image.Jpeg);

            return File(System.IO.File.ReadAllBytes(Path.Combine(_hostingEnvironment.WebRootPath, "Images", isItemImg ? "NoItemImage.png" : "NoImageForPO.png")), System.Net.Mime.MediaTypeNames.Image.Jpeg);
        }
        public IActionResult DownloadTemplate(string id)
        {
            string fileName = id + ".xlsx";
            string templateFile = _commonHelper.GetConstant(Helpers.Constant.TEMPLATES) + fileName;
            string filePath = Path.Combine(templateFile);

            if (System.IO.File.Exists(filePath))
                return PhysicalFile(filePath, MediaTypeNames.Application.Octet, fileName);

            return Content("Template does not exist. Please contact application support team.", MediaTypeNames.Text.Plain);
        }
        public async Task<List<DropDownModel>> ReadMyShops()
        {
            if (_appUser.IsPowerUser)
                return await _Webcontext.Shops.Where(x => x.CompanyId == _appUser.CompanyId).Select(x => new DropDownModel { Id = x.Id, Text = (x.ShopName + " - " + x.ShopCode) }).ToListAsync();

            return await (from se in _Webcontext.ShopEmployees
                          join s in _Webcontext.Shops on se.ShopId equals s.Id
                          where se.EmployeeId == _appUser.EmployeeId
                          select new DropDownModel { Id = s.Id, Text = (s.ShopName + " - " + s.ShopCode) }).ToListAsync();
        }
        public async Task<List<GSTSetting>> ReadGSTSettings()
        {
            return await _Webcontext.GSTSettings.Select(x => new GSTSetting { Id = x.Id, Name = x.Name, Percentage = x.Percentage, GSTInAccountId = x.GSTInAccountId, GSTOutAccountId = x.GSTOutAccountId }).ToListAsync();
        }
        public async Task<List<DropDownModel>> ReadAccountNames()
        {
            return await (from a in _Webcontext.Accounts
                          join dt in _Webcontext.AccountDetailTypes on a.AccountDetailTypeId equals dt.Id
                          join t in _Webcontext.AccountTypes on dt.AccountTypeId equals t.Id

                          where t.CompanyId == _appUser.CompanyId
                          select new DropDownModel { Id = a.Id, Text = string.Concat(a.AccountName, " - ", a.Code) }).ToListAsync();
        }
        public async Task<List<DropDownModel>> ReadCreditAccountNames()
        {
            return await (from a in _Webcontext.Accounts
                          join dt in _Webcontext.AccountDetailTypes on a.AccountDetailTypeId equals dt.Id
                          join t in _Webcontext.AccountTypes on dt.AccountTypeId equals t.Id
                          where t.CompanyId == _appUser.CompanyId
                          && a.AccountName.Contains("Payable")
                          select new DropDownModel { Id = a.Id, Text = string.Concat(a.AccountName, " - ", a.Code) }).ToListAsync();
        }
        public async Task<object> ReadAccountsWithCurrency()
        {
            return await (from a in _Webcontext.Accounts
                          join dt in _Webcontext.AccountDetailTypes on a.AccountDetailTypeId equals dt.Id
                          join at in _Webcontext.AccountTypes on dt.AccountTypeId equals at.Id
                          join cr in _Webcontext.Currencies on a.Currency equals cr.Currency
                          where at.CompanyId == _appUser.CompanyId
                          select new
                          {
                              a.Id,
                              Text = string.Concat(a.AccountName, " - ", a.Code),
                              a.Currency,
                              cr.ConversionRate
                          }).ToListAsync();
        }
        public async Task<List<SupplierInvoice>> ReadAllSupplierInvoices()
        {
            return await _Webcontext.SupplierInvoices.Where(x => x.Active
                                                        && (_appUser.IsPowerUser || x.ShopId == _appUser.ShopId)
                                                        && (x.POSelected || x.ExpenseCategory == "Invoice")
                                                        && !_Webcontext.PurchaseReturnMasters.Any(y => y.SupplierInvoiceId == x.Id && y.Active))
                .Select(x => new SupplierInvoice { Id = x.Id, RefNo = x.RefNo, Amount = x.Amount, ExpenseCategory = x.ExpenseCategory, ShipmentId = x.ShipmentId }).ToListAsync();
        }
        public async Task<List<DropDownModel>> ReadCustomers()
        {
            return await _Webcontext.Customers.Where(x => x.CompanyId == _appUser.CompanyId).Select(x => new DropDownModel { Id = x.ID, Text = x.NameEN + " - " + x.NicNo }).ToListAsync();
        }
    }
}
