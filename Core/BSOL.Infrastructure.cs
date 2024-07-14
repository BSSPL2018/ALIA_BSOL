namespace BSOL.Core
{
    public enum ItemType
    {
        Goods,
        Service,
        Assets,
        Others,
        Consignment
    }
    public enum ImagePath
    {
        ItemImage
    }
    public enum ActionType
    {
        Add,
        Edit,
        Delete,
        Approve,
        Reject,
        Import,
        Export,
        Receive,
        UnReceive,
        Authenticate,
        Cancel,
        UnCancel,
        Undo
    }
    public enum QuotationType
    {
        Cash,
        Credit
    }
    public enum QuotationClosedReason
    {
        Invoiced,
        Declined
    }

}
