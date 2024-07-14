﻿using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Models.Accounts
{
    public class SupplierPaymentModel
    {
        #region columns
        public long Id { get; set; }
        public long SupplierId { get; set; }
        public long PaymentRequestID { get; set; }
        public string Currency { get; set; }
        public string PaymentCategory { get; set; }
        public string PaymentMode { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string ChequeNo { get; set; }
        public string BankName { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string PaidBy { get; set; }
        public string Notes { get; set; }
        public string RefNoFormatted { get; set; }
        public string ReceiptNo { get; set; }
        public long? DebitAccountId { get; set; }
        public long? CreditAccountId { get; set; }
        public long? ShopId { get; set; }
        public string ShopName { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? VerifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string VoucherType { get; set; }
        public string HandedOverTo { get; set; }
        public Boolean CanApprove { get; set; }
        public Boolean CanUndo { get; set; }
        public string AuthorizedBy { get; set; }
        public DateTime? AuthorizedOn { get; set; }
        #endregion
    }
}
