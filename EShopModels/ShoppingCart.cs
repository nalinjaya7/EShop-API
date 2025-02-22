using EShopModels.Common;

namespace EShopModels
{
    public partial class ShoppingCart : BaseEntity, IShoppingCart
    {
        public ShoppingCart() { ShoppingCartItems = new List<ShoppingCartItem>(); }

        public ShoppingCart(int userID, decimal grossAmount, decimal discountAmount, decimal taxAmount)
        { 
            UserID = userID;
            GrossAmount = grossAmount;
            DiscountAmount = discountAmount;
            TaxAmount = taxAmount;
            ShoppingCartItems = new List<ShoppingCartItem>();
        } 

        public int UserID { get; set; }
        public decimal GrossAmount { get;set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public ShoppingCartStatus ShoppingCartStatus { get; set; }
        public EShopUser User { get; set; }
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }

        public void AddItem(ShoppingCartItem item)
        {
            ShoppingCartItems.Add(item);
            GrossAmount = ShoppingCartItems.Sum(a => (a.Quantity * a.UnitPrice));
            DiscountAmount = ShoppingCartItems.Sum(a => a.LineDiscount);
        }
    }

    public interface IShoppingCart
    {
        void AddItem(ShoppingCartItem item);
    }
}
