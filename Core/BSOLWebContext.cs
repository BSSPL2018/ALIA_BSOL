using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOL.Helpers;
using BSOL.Core.Models.Common;
using BSOL.Core.Models.Logitics;
using BSOL.Core.Entities;
using BSOL.Core.Models.HirePurchase;
using BSOL.Core.Models.Warranty;
using BSOL.Core.Models.Promotion;
using BSOL.Core.Models.Marketting;
using BSOL.Core.Models.Sales;
using DocumentFormat.OpenXml.Bibliography;
using System.Diagnostics.Metrics;
using City = BSOL.Core.Entities.City;
using BSOL.Core.Models.Accounts;
using Constant = BSOL.Core.Entities.Constant;
using Department = BSOL.Core.Entities.Department;
using DocumentFormat.OpenXml.Wordprocessing;
using Document = BSOL.Core.Entities.Document;
using Bank = BSOL.Core.Entities.Bank;
using Category = BSOL.Core.Entities.Category;
using DetailedValidationModel = BSOL.Core.Models.General.DetailedValidationModel;
using BSOL.Core.Models.Procurement;
using Notification = BSOL.Helpers.Notification;
using BSOL.Core.Models.Shipment;
using BSOL.Controllers;

namespace BSOL.Core
{
    public partial class BSOLWebContext : DbContext
    {
        public BSOLWebContext(DbContextOptions<BSOLWebContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoginUser>().ToTable("Users");
            modelBuilder.Entity<ShopItem>().HasKey(x => new { x.ShopId, x.ItemId });
            modelBuilder.Entity<ShopEmployee>().HasKey(x => new { x.ShopId, x.EmployeeId });
            modelBuilder.Entity<StockClubbingItem>().HasKey(x => new { x.StockClubbingId, x.ItemId });
            modelBuilder.Entity<ShipmentPurchaseOrderDetail>().HasKey(x => new { x.ShipmentId, x.PurchaseOrderDetailId });
            modelBuilder.Entity<SupplierInvoiceDetail>().HasKey(x => new { x.SupplierInvoiceId, x.PurchaseOrderId });
            modelBuilder.Entity<ItemCosting>().HasKey(x => new { x.ShipmentId, x.ItemId });
            modelBuilder.Entity<SupplierCode>().HasKey(x => new { x.SupplierId, x.ItemId });
            modelBuilder.Entity<CustomerSellingCode>().HasKey(x => new { x.CustomerId, x.ItemId });
            modelBuilder.Entity<StockAdjustmentDetail>().HasKey(x => new { x.StockAdjustmentId, x.ItemId });
            modelBuilder.Entity<PurchaseReturnMaster>().ToTable("PurchaseReturnMaster");

            /* For Stored Procedures */
            modelBuilder.Entity<User>().HasNoKey();
            modelBuilder.Entity<ChartOfAccountModel>().HasNoKey();
            modelBuilder.Entity<SupplierPayment>().HasNoKey();
            modelBuilder.Entity<PaymentRequestValue>().HasNoKey();
            modelBuilder.Entity<ValidationModel>().HasNoKey();
            modelBuilder.Entity<RemittanceModel>().HasNoKey();
            modelBuilder.Entity<DropDownModel>().HasNoKey();
            modelBuilder.Entity<StringTypeModel>().HasNoKey();
            modelBuilder.Entity<ChequeListModel>().HasNoKey();
            modelBuilder.Entity<ChequeList>().HasNoKey();
            modelBuilder.Entity<RemittanceDetailsModel>().HasNoKey();
            modelBuilder.Entity<ChequeDetailsModel>().HasNoKey();
            modelBuilder.Entity<CustomDetailModel>().HasNoKey();
            modelBuilder.Entity<ChequeDepositDateChangeModel>().HasNoKey();
            modelBuilder.Entity<ChequeLockerDetailModel>().HasNoKey();
            modelBuilder.Entity<ChequeLockerDetail>().HasNoKey();
            modelBuilder.Entity<SettlementDetailsModel>().HasNoKey();
            modelBuilder.Entity<ChequeHanedOverModel>().HasNoKey();
            modelBuilder.Entity<SparePartsRequestModel>().HasNoKey();
            modelBuilder.Entity<SparePartsRequest>().HasNoKey();
            modelBuilder.Entity<SparePartsDeliveryStausModel>().HasNoKey();
            modelBuilder.Entity<IntegerTypeModel>().HasNoKey();
            modelBuilder.Entity<DateTimeTypeModel>().HasNoKey();
            modelBuilder.Entity<DetailedValidationModel>().HasNoKey();
            modelBuilder.Entity<PurchaseItemModel>().HasNoKey();
            modelBuilder.Entity<PurchaseReturnDetail>().HasNoKey();
            modelBuilder.Entity<PurchaseReturn>().HasNoKey();
            modelBuilder.Entity<Notification>().HasNoKey();
            modelBuilder.Entity<ItemCostingMasterModel>().HasNoKey();
            modelBuilder.Entity<ItemCostingModel>().HasNoKey();
            modelBuilder.Entity<ShipmentPOSerialModel>().HasNoKey();
            modelBuilder.Entity<SerialedItemModel>().HasNoKey();
            modelBuilder.Entity<PaymentMasterModel>().HasNoKey();
            modelBuilder.Entity<PaymentDetails>().HasNoKey();
            modelBuilder.Entity<SupplierPurchaseOrderModel>().HasNoKey();
            modelBuilder.Entity<PayoutListModel>().HasNoKey();
            modelBuilder.Entity<SupplierPaymentTypeModel>().HasNoKey();
            modelBuilder.Entity<SupplierBankAccountTypeModel>().HasNoKey();
            modelBuilder.Entity<PayoutListDetailsModel>().HasNoKey();
            modelBuilder.Entity<ContractMaster>().HasNoKey();
            modelBuilder.Entity<ContractExpiredList>().HasNoKey();
            modelBuilder.Entity<ContractMasterModel>().HasNoKey();
        }
        public DbSet<LoginUser> Users { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierContact> SupplierContacts { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<AccountDetailType> AccountDetailTypes { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<InvoiceType> InvoiceTypes { get; set; }
        public DbSet<GSTSetting> GSTSettings { get; set; }
        public DbSet<AccountSetting> AccountSettings { get; set; }
        public DbSet<Curency> Currencies { get; set; }
        public DbSet<Constant> Constants { get; set; }
        public DbSet<SupplierPayment> SupplierPayments { get; set; }
        public DbSet<PaymentRequest> PaymentRequest { get; set; }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Employment> Employments { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Designtion> Designations { get; set; }

        public DbSet<Bank> Banks { get; set; }
        public DbSet<CompanyAccount> CompanyAccounts { get; set; }
        public DbSet<ShopGroup> ShopGroups { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<QuotationDetail> QuotationDetails { get; set; }
        public DbSet<ShopCounter> ShopCounters { get; set; }

        public DbSet<CustomerModel> CustomerModels { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<LockerSetting> LockerSettings { get; set; }
        public DbSet<Atoll> Atolls { get; set; }
        public DbSet<Island> Islands { get; set; }
        public DbSet<BounceChequeReason> BounceChequeReasons { get; set; }
        public DbSet<EventLog> EventLogs { get; set; }
        public DbSet<CallLog> CallLogs { get; set; }
        public DbSet<SparePartsRequest> SparePartsRequests { get; set; }
        public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<StockAdjustment> StockAdjustments { get; set; }
        public DbSet<ShopItem> ShopItems { get; set; }
        public DbSet<SellingItemUnit> SellingItemUnits { get; set; }
        public DbSet<UnitOfMeasures> UnitOfMeasures { get; set; }
        public DbSet<UnitConversions> UnitConversions { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<SerialedItem> SerialedItems { get; set; }
        public DbSet<StockClubbing> StockClubbings { get; set; }
        public DbSet<StockRepacking> StockRepackings { get; set; }
        public DbSet<CustomerSellingCode> CustomerSellingCodes { get; set; }
        public DbSet<ShipmentPurchaseOrderDetail> ShipmentPurchaseOrderDetails { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<PaymentRequestACC> PaymentRequestACCS { get; set; }
        public DbSet<HSNSetting> HSNSettings { get; set; }
        public DbSet<ShipmentExpense> ShipmentExpenses { get; set; }
        public DbSet<ShipmentInvoice> ShipmentInvoices { get; set; }
        public DbSet<SupplierInvoice> SupplierInvoices { get; set; }
        public DbSet<StockClubbingItem> StockClubbingItems { get; set; }
        //public DbSet<SupplierInvoiceDetail> SupplierInvoiceDetails { get; set; }
        //public DbSet<CustomerReceipt> CustomerReceipts { get; set; }
        //public DbSet<InvoiceReceipt> InvoiceReceipts { get; set; }
        //public DbSet<InvoicePayment> InvoicePayments { get; set; }
        public DbSet<ItemCosting> ItemCostings { get; set; }
        public DbSet<SalesCategory> SalesCategories { get; set; }
        public DbSet<SupplierCode> SupplierItemCodes { get; set; }
        public DbSet<SupplierInvoiceDetail> SupplierInvoiceDetails { get; set; }
        public DbSet<ShopEmployee> ShopEmployees { get; set; }
        public DbSet<ShipmentExpenseCategory> ShipmentExpenseCategories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<EmployeeDetail> EmployeeDetails { get; set; }
        public DbSet<PurchaseReturnMaster> PurchaseReturnMasters { get; set; }
        public DbSet<PurchaseReturnDetail> PurchaseReturnDetails { get; set; }
        public DbSet<Port> Ports { get; set; }
        public DbSet<FollowupNotification> FollowupNotifications { get; set; }
        public DbSet<FollowUp> FollowUps { get; set; }
        public DbSet<SupplierBankDetail> SupplierBankDetails { get; set; }
        public DbSet<SupplierServiceDetail> SupplierServiceDetails { get; set; }
        public DbSet<PaymentMaster> PaymentMasters { get; set; }
        public DbSet<PaymentDetails> PaymentDetails { get; set; }
        public DbSet<BusinessService> BusinessServices { get; set; }
        public DbSet<ItemCategorySetting> ItemCategorySettings { get; set; }
        public DbSet<Models.Accounts.ContractMaster> ContractMasters { get; set; }
        public DbSet<Models.Accounts.ContractExpiredList> ContractExpiredLists { get; set; }
        public DbSet<TravelAgent> TravelAgents { get; set; }
    }
}
