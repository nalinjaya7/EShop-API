using EShopModels;
using EShopModels.Repository;
using Microsoft.EntityFrameworkCore;

namespace EShopRepository.Repositories
{
    public class InventoryRepository : BaseRepository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<object> GetInventoriesForSearchAsync(Inventory inventory)
        {
            if(inventory.ProductID == 0)
            {
                return await Task.FromResult<object>(await Set.Include(h => h.Product).Include(g => g.UnitChart).OrderBy(f => f.ID).ToListAsync<Inventory>());
            }
            else if (inventory.ProductID == 0)
            {
                return await Task.FromResult<object>(await Set.Include(h => h.Product).Include(g => g.UnitChart).OrderBy(f => f.ID).ToListAsync<Inventory>());
            }
            else if (inventory.ProductID != 0)
            {
                return await Task.FromResult<object>(await Set.Include(h => h.Product).Include(g => g.UnitChart).Where(p=>p.ProductID == inventory.ProductID).OrderBy(f => f.ID).ToListAsync<Inventory>());
            }
            else
            {
                return await Task.FromResult<object>(await Set.Include(h => h.Product).Include(g => g.UnitChart).Where(v=>v.ProductID == inventory.ProductID).OrderBy(f => f.ID).ToListAsync<Inventory>());
            }
        }

        public async Task<object> SearchAsync(object obj)
        {
           return await Task.FromResult<object>(await Set.Include(h => h.Product).Include(g => g.UnitChart).OrderBy(f => f.ID).ToListAsync<Inventory>());
         }
    }
}
