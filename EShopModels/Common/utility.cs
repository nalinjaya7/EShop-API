namespace EShopModels.Common
{
    public enum Roles : Int32
    {
        User = 0,
        Admin = 1
    }

    public enum InventoryUpdateType : int
    {
        Sales = 0,
        Return = 1,
        PurchaseOrder = 2,
        StockTransfer = 3

    }

    public enum SearchBoxType : int
    {
        Product = 0,
        SubCategory = 1
    }

    public enum InvoiceStatus : int
    {
        None = 0,
        UnPaid = 1,
        Partial = 2,
        Completed = 3,
        Return = 4,
        Cancelled = 5
    }

    public enum PaymentType : int
    {
        None = 0,
        Cash = 1,
        Card = 2,
        Cheque = 3
    }

    public enum OrderStatus : int
    {
        None = 0,
        Order = 1,
        SalesOrder = 2,
        Cancelled = 3
    }

    public enum ShoppingCartStatus : int
    {
        Pending= 0,
        Completed=1
    }
  
    public enum PromotionApplyType : int
    {
        Quantity = 0,
        Value = 1
    }

    public enum DayType : int
    {
        EveryDay = 0,
        WeekDay = 1
    }

    public enum DiscountType : int
    {
        Amount = 0,
        Percentage = 1,
        Quantity = 2
    }
 
}


