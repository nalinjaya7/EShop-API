using EShopModels;
using EShopModels.Repository;
using EShopModels.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShopServices.ServicesAddNewShoppingCartItemAsync
{
    public class ShoppingCartItemService : BaseService<ShoppingCartItem>,IShoppingCartItemService
    {
        private readonly IShoppingCartItemRepository _shoppingCartItemRepository;

        public ShoppingCartItemService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _shoppingCartItemRepository = unitOfWork.ShoppingCartItemRepository;
        }

        public async Task AddNewShoppingCartItemAsync(ShoppingCartItem cartItem, int UserID)
        {
            await _shoppingCartItemRepository.AddNewShoppingCartItemAsync(cartItem, UserID);            
        }         

        public async Task<object> DeleteCartItemAsync(int id)
        {
           return await _shoppingCartItemRepository.DeleteCartItemAsync(id);
        }

        public async Task<object> UpdateCartItemQtyAsync(ShoppingCartItem shoppingCart)
        {
            return await _shoppingCartItemRepository.UpdateCartItemQtyAsync(shoppingCart);
        }
    }
}
