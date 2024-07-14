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
using DocumentFormat.OpenXml.Bibliography;
using System.Diagnostics.Metrics;
using City = BSOL.Core.Entities.City;
using BSOL.Core.Models.DashBoard;

namespace BSOL.Core
{
    public partial class BSOLContext : DbContext
    {
        public BSOLContext(DbContextOptions<BSOLContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdvanceBooking>().ToTable("tblPOS_UnitAdvanceBooking").HasKey(x => x.BookingID);
            modelBuilder.Entity<Agreement>().ToTable("tblHP_Agreement_Details").HasKey(x => x.tbl_Id);
            modelBuilder.Entity<CreditEvaluation>().ToTable("tblHP_CreditEvaluation").HasKey(x => x.tblEvl_ID);
            modelBuilder.Entity<CustomerDetail>().ToTable("tblHP_CustomerDetails").HasKey(x => x.tbl_ID);
            modelBuilder.Entity<WebProforma>().ToTable("tblPOS_WebProforma").HasKey(x => x.ID);
            modelBuilder.Entity<BillingMaster>().ToTable("tblPOS_BillingMaster").HasKey(x => x.tbl_Id);
            modelBuilder.Entity<AgreementList>().ToTable("tblAgreementList").HasKey(x => x.ID);
            modelBuilder.Entity<ShipmentSerialNo>().ToTable("tblShipmentSerialNos").HasKey(x => x.sno);
            modelBuilder.Entity<ItemCategoryBSOL>().ToTable("tblItemCategory").HasKey(x => x.ItemCategory_ID);

            /* For Stored Procedures */
            modelBuilder.Entity<User>().HasNoKey();
            modelBuilder.Entity<LoggedInUserModel>().HasNoKey();
            modelBuilder.Entity<LorryPaymentModel>().HasNoKey();
            modelBuilder.Entity<BundleValue>().HasNoKey();
            modelBuilder.Entity<StringTypeModel>().HasNoKey();
            modelBuilder.Entity<DropDownModel>().HasNoKey();
            modelBuilder.Entity<UserRightModel>().HasNoKey();
            modelBuilder.Entity<AgreementListModel>().HasNoKey();
            modelBuilder.Entity<WarrantyDetail>().HasNoKey();
            modelBuilder.Entity<ValidationModel>().HasNoKey();
            modelBuilder.Entity<CourtCaseListModel>().HasNoKey();
            modelBuilder.Entity<CustomerFollowup>().HasNoKey();
            modelBuilder.Entity<LorryRate>().HasNoKey();
            modelBuilder.Entity<LorryPaymentSettingModel>().HasNoKey();
            modelBuilder.Entity<DetailedValidationModel>().HasNoKey();
            modelBuilder.Entity<PromotionCustomer>().HasNoKey();
            modelBuilder.Entity<RamadanPromotionModel>().HasNoKey();
            modelBuilder.Entity<ChequeDepositDateChangeModel>().HasNoKey();
            modelBuilder.Entity<SalesStatus>().HasNoKey();
            modelBuilder.Entity<MonthlyHPStatusModel>().HasNoKey();
            modelBuilder.Entity<UnitStatus>().HasNoKey();
        }

        public DbSet<Agreement> Agreements { get; set; }
        public DbSet<AdvanceBooking> AdvanceBookings { get; set; }
        public DbSet<CreditEvaluation> CreditEvaluations { get; set; }
        public DbSet<CustomerDetail> CustomerDetails { get; set; }
        public DbSet<WebProforma> WebProformas { get; set; }
        public DbSet<BillingMaster> BillingMasters { get; set; }
        public DbSet<AgreementList> AgreementLists { get; set; }
        public DbSet<RamazanPromotion> RamazanPromotions { get; set; }
        public DbSet<ChequeDepositDateChange> ChequeDepositDateChanges { get; set; }
        public DbSet<ShipmentSerialNo> ShipmentSerialNos { get; set; }
        public DbSet<ItemCategoryBSOL> ItemCategorys { get; set; }
    }
}
