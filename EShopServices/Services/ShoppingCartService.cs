using EShopModels;
using EShopModels.Repository;
using EShopModels.Services;
using EShopRepository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShopServices.Services
{
    public class ShoppingCartService : BaseService<ShoppingCart>,IShoppingCartService
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public ShoppingCartService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _shoppingCartRepository = unitOfWork.ShoppingCartRepository;
        }

        public async Task<object> GetShoppingCartAsync(int userID)
        {
            return await _shoppingCartRepository.GetShoppingCartAsync(userID);
        }
    }
}
