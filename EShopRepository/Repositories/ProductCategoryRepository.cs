using EShopModels;
using EShopModels.Repository;
using Microsoft.EntityFrameworkCore;

namespace EShopRepository.Repositories
{
    public class ProductCategoryRepository : BaseRepository<ProductCategory>, IProductCategoryRepository
    {
        public ProductCategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<object> GetAllSearchBoxItemsAsync()
        {
            List<SearchBox> searchBoxItems = new List<SearchBox>();
            var products = await _context.Set<Product>().Include(k=>k.Inventories).ThenInclude(t=>t.UnitChart).Select(g => new SearchBox() { ID = g.ID, Name = g.Name, Type = EShopModels.Common.SearchBoxType.Product,Prices = g.Inventories.Select(u=>new Price() { AvailableQty = u.Quantity, UnitPrice=u.SellingPrice,UnitName=u.UnitChartName}).ToList() }).ToListAsync();
            searchBoxItems.AddRange(products);
 
            return await Task.FromResult<List<SearchBox>>(searchBoxItems);
        }

        public async Task<object> GetSearchITemDetailAsync(int id)
        {
            return await Task.FromResult<object>(await _context.Set<Product>().Where(h => h.ID == id).FirstOrDefaultAsync<Product>());            
        }

        public async Task<object> SearchAsync(object obj)
        { 
           return await Task.FromResult<List<ProductCategory>>(await Set.OrderBy(j => j.Name).ToListAsync());  
        }

        public virtual async Task<object> SelectAllAsync()
        { 
            List<ProductCategory> productCategories = new List<ProductCategory>();
            ProductCategory category = new ProductCategory("Foods");
            category.ProductSubCategories = new List<ProductSubCategory>();
            category.ID = 1;
            category.ProductSubCategories.Add(new ProductSubCategory(1, "Fresh Vegitable") { ID = 1 });
            category.ProductSubCategories.Add(new ProductSubCategory(1, "Sea Foods") { ID = 13 });
            productCategories.Add(category);
            return await Task.FromResult<object>(productCategories);

            // return await Task.FromResult<object>(await Set.Include(t=>t.ProductSubCategories).ToListAsync());
        }

        public async Task<int> UpdateAsync(int ID, object obj)
        {
           _context.Entry((ProductCategory)obj).State = EntityState.Modified;
            return await Task.FromResult<int>(await _context.SaveChangesAsync());
        }
    }
}
