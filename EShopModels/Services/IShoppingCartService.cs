using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShopModels.Services
{
    public interface IShoppingCartService : IService<ShoppingCart>
    {
          Task<object> GetShoppingCartAsync(int userID);
    }
}
