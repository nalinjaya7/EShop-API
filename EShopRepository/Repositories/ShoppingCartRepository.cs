using EShopModels;
using EShopModels.Repository;
using Microsoft.EntityFrameworkCore;

namespace EShopRepository.Repositories
{
    public class ShoppingCartRepository : BaseRepository<ShoppingCart>, IShoppingCartRepository
    {
        public ShoppingCartRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<object> GetShoppingCartAsync(int userID)
        {
            ShoppingCart? obj = await Set.Include(g => g.User).Include(g => g.ShoppingCartItems).ThenInclude(t => t.Product).Include(g => g.ShoppingCartItems).ThenInclude(t => t.UnitChart).Where(g => g.UserID == userID && g.ShoppingCartStatus == EShopModels.Common.ShoppingCartStatus.Pending).FirstOrDefaultAsync();
            return await Task.FromResult<ShoppingCart>((obj == null ? (new ShoppingCart(userID,0,0,0)) : obj));
        }
    }
}
