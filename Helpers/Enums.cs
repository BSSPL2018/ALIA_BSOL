using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Helpers
{
    public enum ClaimType
    {
        EmployeeId,
        ShortName,
        FullName,
        IsPowerUser,
        Rights,
        UserCode,
        CompanyId,
        UserID,
        CommanName,
        PrimaryCurrency,
        ConversionRate,
        UniqueID,
        IsGuest,
        ID,
        ShopId,
        ShopName,
        ShopGST,
        ShopGSTInAccountId,
        ShopGSTOutAccountId,
        ShopCount,
        ShopCode,
        ServiceCharge,
        EntityID,
        BaseCurrency,
        IsAdmin,
        AppUserName
    }
    public enum ShipmentMode
    {
        Sea,
        Air
    }

    public enum ContainerLoadType
    {
        FCL,
        LCL
    }
    public enum ClearanceType
    {
        Normal,
        Bonded
    }
    public enum Rights
    {
        Full,
        View,
        Add,
        Edit,
        Modify,
        Delete,
        Approve,
        Reject
    }

    public enum Format
    {
        [Description("dd-MMM-yyyy")]
        Date,
        [Description("HH:mm")]
        Time,
        [Description("dd-MMM-yyyy hh:mm tt")]
        DateTime,
        [Description("MMM-yyyy")]
        Month
    }
    public enum ExcelTemplate
    {
        ItemRegistration,
        InvoiceItems,
        StockAdjustment
    }
    public enum Orientation
    {
        Horizontal,
        Vertical
    }
    public enum ReportType
    {
        Report,
        Excel
    }
    public enum DocumentReference
    {
        Suppliers,
        [Description("Accounts-SupplierPayment")]
        SupplierPayment,
        [Description("Accounts-TicketRequest")]
        TicketRequest,
        [Description("Accounts-Deductions")]
        Deductions,
        [Description("Accounts-OfficialTrip")]
        OfficialTrip,
        [Description("Accounts-PettyCash")]
        PettyCash,
        [Description("Accounts-ExpatriateProcess")]
        ExpatriateProcess,
        [Description("Accounts-HP")]
        HP,
        [Description("Accounts-SHIPMENT")]
        SHIPMENT,
        [Description("Accounts-REMITTANCE")]
        Remittance,
        [Description("POS-SpareParts")]
        SpareParts,
        [Description("PurchaseReturns")]
        PurchaseReturns,
        [Description("PurchaseReturns")]
        StockAdjustments,
        [Description("Shipments")]
        Shipments,
        [Description("PurchaseOrders")]
        PurchaseOrders,
        [Description("SupplierInvoices")]
        SupplierInvoices,
        [Description("Quotations")]
        Quotations,
        [Description("PolicyContract")]
        PolicyContract,
        [Description("Accounts-PaymentReturn")]
        PaymentReturn,
    }
    public enum ReceiptPaymentMode
    {
        [Description("Cash")]
        Cash,
        [Description("Cheque")]
        Cheque,
        [Description("Remittance")]
        Remittance,
        [Description("Credit Card")]
        CreditCard,
        [Description("Credit-Cash")]
        CreditCash,
        [Description("Credit-Cheque")]
        CreditCheque,
        [Description("Credit-TT")]
        CreditTT,
        [Description("Bank transfer")]
        BankTransfer,
        [Description("TT")]
        TT
    }
    public enum AccountTypes
    {
        Assets,
        Liabilities,
        Equity,
        Income,
        Expenses
    }
    public enum Constant
    {
        DOCUMENT,
        TEMPLATES,
        ACCOUNTS
    }
    public enum CacheVariable
    {
        Constants
    }

    public enum MessageType
    {
        Email,
        Notification,
        SMS
    }
    public enum PurchaseOrderPaymentType
    {
        [Description("Cash")]
        Cash,
        [Description("LC")]
        LC,
        //[Description("Credit")]
        //Credit,
        [Description("Credit-Cash")]
        CreditCash,
        [Description("Credit-Cheque")]
        CreditCheque,
        [Description("Credit-DT")]
        CreditDT,
        [Description("Cheque")]
        Cheque,
        [Description("Open Account")]
        OpenAccount,
        [Description("Advance")]
        Advance
    }
    public enum ItemType
    {
        Goods,
        Service,
        Assets,
        Others,
        Consignment,
        Unit,
        Parts
    }
    public enum PaymentCategory
    {
        //[Description("With PO Invoice")]
        //WITHPURCHASEORDER,
        //[Description("Without PO Invoice")]
        //WITHOUTPOINVOICE,
        //[Description("Without Purchase Order")]
        //WITHOUTPURCHASEORDER,
        [Description("Supplier Invoice")]
        SUPPLIERINVOICE,
        [Description("Purchase Order")]
        PURCHASEORDER,
        [Description("Shipment")]
        SHIPMENT,
        [Description("Sales Return")]
        SALESRETURN,
    }
    public enum StaffPaymentCategory
    {
        //[Description("With PO Invoice")]
        //WITHPURCHASEORDER,
        //[Description("Without PO Invoice")]
        //WITHOUTPOINVOICE,
        //[Description("Without Purchase Order")]
        //WITHOUTPURCHASEORDER,
        [Description("Supplier Invoice")]
        SUPPLIERINVOICE,
        [Description("Purchase Order")]
        PURCHASEORDER,
        [Description("Shipment")]
        SHIPMENT,
        [Description("Sales Return")]
        SALESRETURN,
        [Description("Staff Reimbursement")]
        STAFFREIMBURSEMENT,
        [Description("Staff Payment")]
        STAFFPAYMENT
    }
    public enum PayeeType
    {
        Supplier,
        Staff,
        Customer,
        //Service,
        //Ticket
    }
    public enum SupplierType
    {
        Local,
        International
    }
}
