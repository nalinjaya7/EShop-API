using EShopModels.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace EShopModels
{
    public partial class ShoppingCartItem : BaseEntity
    {
        public ShoppingCartItem(int shoppingCartID, int productID,int unitChartID, decimal unitPrice, decimal lineDiscount, decimal quantity)
        { 
            ShoppingCartID = shoppingCartID;
            ProductID = productID;
            UnitChartID = unitChartID;
            UnitPrice = unitPrice;
            LineDiscount = lineDiscount;
            Quantity = quantity; 
        }

        [Required]
        public int ShoppingCartID { get; set; }
        [Required]
        public int UnitChartID { get; set; }
        [Required]
        public int ProductID { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
        [Required]
        public decimal LineDiscount { get; set; }
        [Required]
        [Range(minimum:1,maximum:1000000,ErrorMessage="Invalid Quantity")]
        public decimal Quantity { get; set; }
        [ValidateNever]
        public ShoppingCart ShoppingCart { get; set; }
        [ValidateNever]
        public Product Product { get; set; }
        [ValidateNever]
        public UnitChart UnitChart { get; set; }
    }
}
