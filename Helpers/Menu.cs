using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace BSOL.Helpers
{
    public class Menu
    {
        private string[] _rights = new List<string>().ToArray();
        private bool _isPowerUser;
        public string MenuName { get; set; }
        public string ClassName { get; set; }
        public string Color { get; set; }
        public string Url { get; set; }
        public string Form { get; set; }

        private Menu[] _menuItems;
        public Menu[] MenuItems
        {
            get { return _menuItems ?? new List<Menu>().ToArray(); }
            set { _menuItems = value; }
        }

        public Menu()
        {
        }

        public Menu(string menuName, string url, string form = "", string icon = "", string color = "")
        {
            this.MenuName = menuName;
            this.Url = url;
            this.Form = form;
            this.ClassName = icon;
            this.Color = color;
        }

        private Menu TabItems(string mainMenu, List<Menu> tabs, string icon = "", string color = "")
        {
            if (!tabs.Any())
                return null;
            this.Color = color;
            this.ClassName = icon;
            var firstTab = tabs.FirstOrDefault();
            return new Menu(mainMenu, firstTab.Url, firstTab.Form, icon, color) { MenuItems = tabs.ToArray() };
        }

        public IEnumerable<Menu> Get(IHtmlHelper helper)
        {
            if (helper.ViewData["MENUS"] != null)
                return (IEnumerable<Menu>)helper.ViewData["MENUS"];

            Init(helper);

            /* https://fonts.google.com/icons */

            var menus = new Menu[]{
                    Validate(new Menu{
                        MenuName = "Material Management", ClassName = "shopping_cart_checkout", Color = "#f4b400",
                        MenuItems = Validate(new Menu[]{
                                         new Menu("Item Registration","/Procurements/Items",Forms.Procurement.Items, "universal_currency_alt", "#e7501a"),
                                         new Menu().TabItems("Supplier", Validate(new Menu[]{
                                            new Menu("Supplier Registration","/Procurements/Suppliers",Forms.Procurement.Supplier,"person_add","#f4b400")
                                         }),"person_add","#f4b400"),
                                         new Menu().TabItems("Purchase", Validate(new Menu[]{
                                            new Menu("Purchase Order","/Procurements/PurchaseOrder", Forms.Procurement.Purchase, "dataset_linked", "#dd3f39"),
                                            new Menu("Purchase Return", "/Procurements/PurchaseReturn", Forms.Procurement.PurchaseReturn, "receipt", "#dd3f39")
                                         }),"dataset_linked", "#dd3f39"),
                                         new Menu().TabItems("Shipments", Validate(new Menu[]{
                                            new Menu("Shipments", "/Shipment/Shipments",Forms.Procurement.Shipments,"package", "#D2EC1A"),
                                            new Menu("Serial Items", "/Shipment/SerialedItems",Forms.Procurement.Shipments,"package", "#00C9A7")
                                         }),"package", "#00C9A7"),
                                         new Menu("Payments", "/Accounts/PaymentRequest",Forms.Accounts.PaymentRequest,"payments", "#00C9A7"),
                                         //new Menu().TabItems("Shops", Validate(new Menu[]{
                                         new Menu().TabItems("Business Unit", Validate(new Menu[]{
                                            new Menu("Shop","/Shop/Shops",Forms.Procurement.Shops,"diversity_3","#fd636b"),
                                            new Menu("Shop Employees","/Shop/ShopEmployees",Forms.Procurement.Shops,"person_add","#f4b400")
                                         }),"diversity_3","#fd636b"),
                                         new Menu("Item Costing", "/Procurements/ItemCosting",Forms.Procurement.Costing,"account_balance_wallet", "#FD0FF9"),
                                         new Menu("Receive Item", "/Stock/PerishableItems",Forms.Procurement.ReceiveItems,"package", "#f4b400"),
                                         new Menu().TabItems("Control Panel", Validate(new Menu[]{
                                            new Menu("Unit Of Measures","/Procurements/UnitOfMeasures",Forms.Procurement.Items, "dataset_linked", "#dd3f39"),
                                            new Menu("Currency/HSN Setting","/Procurements/HSNSettings",Forms.Procurement.ControlPanel, "construction", "#00C9A7"),
                                            new Menu("Port Setting","/ControlPanel/PortSettings",Forms.Procurement.Shipments, "construction", "#00C9A7"),
                                            new Menu("Item Category Setting","/ControlPanel/ItemCategorySettings",Forms.Procurement.ItemCategorySetting, "lock", "#fd636b")
                                         }),"construction", "#00C9A7"),
                                         new Menu("Lorry Payment","/Logistics/LorryPayment",Forms.Logistics.LorryPayment, "local_shipping", "#dd3f39"),
                                         new Menu("Lorry Payment Setting","/Logistics/LorryPaymentSetting",Forms.Logistics.LorryPaymentSetting, "settings_suggest", "#3adb4c"),
                                         new Menu("Sales Summary","/Sales/SalesSummary",Forms.Logistics.LorryPaymentSetting, "wallet", "#0fa2f0"),
                                         
                                         //new Menu("Quotation Model","/Quotation/Quotations",Forms.Procurement.Quotations,"construction","#f4b400"),
                        }).ToArray()
                    }),

                     Validate(new Menu{
                        MenuName = "Retail", ClassName = "construction", Color = "#79909b",
                        MenuItems = Validate(new Menu[]{
                                        new Menu("Customer","/CRM/Customer",Forms.CRM.Customer,"person_add","#f4b400"),
                                        new Menu("Remittance","/Accounts/Remittance",Forms.Accounts.Remittance, "receipt", "#c8a3ff"),
                                        new Menu("Cheque","/HirePurchase/ChequeList",Forms.HirePurchases.ChequeRegistry, "payments", "#00C9A7"),
                                        new Menu("Settlement","/Accounts/RemittanceDetails",Forms.Accounts.SettlementDetails, "dataset_linked", "#dd3f39"),
                                        new Menu("Due Date Change","/HirePurchase/DepositDateChangeList",Forms.HirePurchases.DepositDateChange, "domain", "#5ef434"),
                                        new Menu("Cheque Locker","/HirePurchase/ChequeSafetyLocker",Forms.HirePurchases.ChequeLocker, "lock", "#ff7a05"),
                                        new Menu("Sales Status","/DashBoard/SalesStatus",Forms.DashBoard.SalesStaus, "universal_currency_alt", "#ccff71"),
                                        new Menu("Spare Parts Request","/Sales/SparePartsRequest",Forms.Sales.SparePartsRequest, "offline_bolt", "#FE378F"),
                                        
                        }).ToArray()
                    }),

                    Validate(new Menu{
                        MenuName = "CRM Suite", ClassName = "diversity_3", Color = "#fd636b",
                        MenuItems = Validate(new Menu[]{
                                         new Menu("Customer","/CRM/Customer",Forms.CRM.Customer,"person_add","#f4b400"),
                                         new Menu("Warranty","/Warranty/WarrantyMaster",Forms.PosAndMarketting.WarrantyMaster,"verified","#FE378F"),
                                         new Menu("Ramadan Promotion","/Marketting/RamadanPromotion",Forms.PosAndMarketting.RamadanPromotion,"point_of_sale","#e7501a")
                        }).ToArray()
                    }),

                    new Menu{
                        MenuName = "Hire Purchase", ClassName = "approval_delegation", Color = "#fbbc05",
                        MenuItems = Validate(new Menu[]{
                                         new Menu().TabItems("Dashboard", Validate(new Menu[]{
                                            new Menu("Sales Status","/DashBoard/SalesStatus",Forms.DashBoard.SalesStaus, "universal_currency_alt", "#e7501a"),
                                            //new Menu("HP Process Status","/HirePurchase/MonthlyUnitStatus",Forms.HirePurchases.HPMonthlyStatus, "offline_bolt", "#FE378F"),
                                            new Menu("Unit Status","/DashBoard/UnitStatus",Forms.HirePurchases.HPMonthlyStatus, "offline_bolt", "#FE378F"),
                                         }),"space_dashboard","#0f9d58"),
                                         new Menu("Customer","/CRM/Customer",Forms.CRM.Customer,"person_add","#f4b400"),
                                         new Menu("Cheque","/HirePurchase/ChequeList",Forms.HirePurchases.ChequeRegistry, "payments", "#00C9A7"),
                                         new Menu("Settlement","/Accounts/RemittanceDetails",Forms.Accounts.SettlementDetails, "dataset_linked", "#dd3f39"),
                                         new Menu("Due Date Change","/HirePurchase/DepositDateChangeList",Forms.HirePurchases.DepositDateChange, "domain", "#38474f"),
                                         new Menu("Cheque Locker","/HirePurchase/ChequeSafetyLocker",Forms.HirePurchases.ChequeLocker, "lock", "#fd636b"),
                                         new Menu("Cheque HandOver","/HirePurchase/ChequeHandOver",Forms.HirePurchases.ChequeRegistry, "real_estate_agent", "#FE378F"),
                                         new Menu("Agreement","/HirePurchase/Agreement",Forms.HirePurchases.Agreement, "handshake", "#4285f4"),
                                         new Menu("Legal Case","/HirePurchase/CourtCaseList",Forms.HirePurchases.LegalCase, "construction", "#00C9A7"),

                        }).ToArray()
                    },

                    Validate(new Menu{
                        MenuName = "Finance", ClassName = "monetization_on", Color = "#0f9d58",
                        MenuItems = Validate(new Menu[]{
                            new Menu("Remittance","/Accounts/Remittance",Forms.Accounts.Remittance, "receipt", "#dd3f39"),
                            new Menu("Cheque","/HirePurchase/ChequeList",Forms.HirePurchases.ChequeRegistry, "payments", "#00C9A7"),
                            new Menu("Settlement","/Accounts/RemittanceDetails",Forms.Accounts.SettlementDetails, "dataset_linked", "#dd3f39"),
                            new Menu("Due Date Change","/HirePurchase/DepositDateChangeList",Forms.HirePurchases.DepositDateChange, "domain", "#42f276"),
                            new Menu("Cheque HandOver","/HirePurchase/ChequeHandOver",Forms.HirePurchases.ChequeRegistry, "real_estate_agent", "#FE378F"),
                            new Menu("Contract Master","/Accounts/Contract",Forms.Accounts.ContractList, "contract", "#42f276"),
                            new Menu("Reports","/Accounts/Reports",Forms.Report.Reports, "report", "#79909b"),
                            //new Menu("Payment Request","/Accounts/RequestedPaymentList",Forms.Accounts.PaymentRequest, "dynamic_form", "#722ab3"),
                            new Menu("Payment Voucher","/Accounts/PaymentVoucher",Forms.Accounts.PaymentVoucher, "account_balance_wallet", "#e7501a"),
                            new Menu().TabItems("Chart of Accounts", Validate(new Menu[]{
                                            new Menu("Chart of Accounts","/Accounts/ChartofAccounts",Forms.Accounts.ChartOfAccounts),
                                            new Menu("Account Settings","/Accounts/AccountTypes",Forms.Accounts.ChartOfAccounts),
                                            new Menu("Invoice Type/GST Settings","/Accounts/InvoiceTypesGSTSettings",Forms.Accounts.ChartOfAccounts)
                                        }),"account_box","#0f9d58")
                        }).ToArray()
                    }),

                    Validate(new Menu{
                        MenuName = "Control Panel", ClassName = "valve", Color = "#0f9d58",
                        MenuItems = Validate(new Menu[]{
                            new Menu("Bank","/Accounts/BankDetails",Forms.Accounts.BankDetails, "account_balance", "#79909b"),
                            new Menu("Cheque Locker","/HirePurchase/ChequeSafetyLocker",Forms.HirePurchases.ChequeLocker, "lock", "#fd636b"),
                            new Menu("Business","/Accounts/BusinessServices",Forms.Accounts.BusinessServices, "business_center", "#95d943")
                        }).ToArray()
                    }),

                    Validate(new Menu{
                        MenuName = "Report", ClassName = "report", Color = "#ea4335",
                        MenuItems = Validate(new Menu[]{
                            new Menu("Reports","/Report/Reports",Forms.Report.Reports, "report", "#79909b")
                        }).ToArray()
                    }),
                    };
            menus = menus.Where(x => x.Url.IsValid() || x.MenuItems.Any()).ToArray();
            menus.Where(x => !x.Url.IsValid()).ToList().ForEach(x => x.Url = x.MenuItems?.Select(x => x.Url).FirstOrDefault());
            helper.ViewData["MENUS"] = menus;
            return menus;
        }

        public IEnumerable<Menu> GetBreadCrumb(IHtmlHelper helper, string parentMenu)
        {
            Init(helper);
            var menus = this.Get(helper);
            if (!parentMenu.IsValid())
                parentMenu = menus.Where(x => helper.ViewContext.HttpContext.Request.Path.ToString().StartsWith(x.Url)).Select(x => x.MenuName).FirstOrDefault();
            parentMenu = parentMenu.IsValid() ? parentMenu : menus.Select(x => x.MenuName).FirstOrDefault();
            var parent = menus.FirstOrDefault(x => x.MenuName == parentMenu);
            return parent == null ? new List<Menu>().ToArray() : parent.MenuItems;
        }

        public IEnumerable<Menu> GetTabs(IHtmlHelper helper, string parentMenu, string childMenu)
        {
            Init(helper);
            var parent = this.Get(helper).FirstOrDefault(x => x.MenuName == parentMenu);
            return parent == null ? new List<Menu>().ToArray() : parent.MenuItems.Length == 0 ? new List<Menu>().ToArray() : (parent.MenuItems.FirstOrDefault(x => x.MenuName == childMenu).MenuItems);
        }

        private void Init(IHtmlHelper helper)
        {
            string data = helper.ViewContext.HttpContext.User.FindFirstValue(ClaimType.Rights.ToString());
            if (data.IsValid())
                _rights = data.Split(',');
            _isPowerUser = Convert.ToBoolean(helper.ViewContext.HttpContext.User.FindFirstValue(ClaimType.IsPowerUser.ToString()));
        }

        private List<Menu> Validate(IEnumerable<Menu> menus)
        {
            return menus.Where(x => x != null && (!x.Form.IsValid() || _isPowerUser || _rights.Any(y => y == x.Form))).ToList();
        }

        private Menu Validate(Menu menu)
        {
            return Validate(new[] { menu }).FirstOrDefault() ?? new Menu();
        }
    }
}
