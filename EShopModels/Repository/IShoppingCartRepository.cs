using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShopModels.Repository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
      Task<object> GetShoppingCartAsync(int userID);
    }
}
