using EShopModels;
using EShopModels.Repository;
using Microsoft.EntityFrameworkCore;

namespace EShopRepository.Repositories
{
    public class ProductSubCategoryRepository : BaseRepository<ProductSubCategory>, IProductSubCategoryRepository
    {
        public ProductSubCategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<object> GetSubCategoriesByCategoryAsync(int CategoryID, int pageNumber)
        {
           return await Task.FromResult<List<ProductSubCategory>>(await Set.Include(d => d.ProductCategory).Where(c => c.ProductCategoryID == CategoryID).OrderBy(j => j.Name).ToListAsync());
        }

        public async Task<object> GetSubCategoriesByCategoryAsync(int categoryID)
        { 
           return await Task.FromResult<List<ProductSubCategory>>(await Set.Where(f => f.ProductCategoryID == categoryID).OrderBy(j => j.Name).ToListAsync());
        }

        public async Task<object> GetSubCategoryByID(int id)
        {
            return await Task.FromResult<object>(await Set.Include(g => g.ProductCategory).FirstOrDefaultAsync(t => t.ID == id));
        }

        public async Task<object> SearchAsync(object obj)
        { 
            return await Task.FromResult<List<ProductSubCategory>>(await Set.Include(u => u.ProductCategory).OrderBy(j => j.Name).ToListAsync());
        }

        public async Task<object> SearchAsync()
        {
          return await Task.FromResult<List<ProductSubCategory>>(await Set.OrderBy(j => j.Name).ToListAsync<ProductSubCategory>());
        }
    }
}
