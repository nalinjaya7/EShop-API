using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShopModels.Services
{
    public interface IShoppingCartItemService : IService<ShoppingCartItem>
    {
        Task AddNewShoppingCartItemAsync(ShoppingCartItem cartItem,int UserID); 
        Task<object> DeleteCartItemAsync(int id);
        Task<object> UpdateCartItemQtyAsync(ShoppingCartItem shoppingCart);
    }
}
