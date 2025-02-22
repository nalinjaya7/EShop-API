using EShopModels;
using EShopModels.Repository;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EShopRepository.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<object> GetByIDAsync(int id)
        {
           return await Task.FromResult<object>(await Set.Include(h => h.ProductSubCategory).Include(u => u.Inventories).Include(i => i.UnitCharts).ThenInclude(y => y.UnitType).Where(y => y.ID == id).SingleOrDefaultAsync());
        }
 
        public async Task<object> GetProductInventoriesAsync(int ProductID)
        {
           return await Task.FromResult<object>(await _context.Inventories.Include(g => g.UnitChart.UnitType).Where(j => j.ProductID == ProductID).ToListAsync());
        }

        public async Task<object> SearchAsync(object obj)
        {
           return await Task.FromResult<List<Product>>(await Set.Include(u => u.ProductSubCategory).OrderBy(j => j.Name).ToListAsync()); 
        }

        public async Task<object> SearchAsync(object obj, Product product)
        {
            product.Name ??= "";
            product.ItemCode ??= "";
            product.ProductSubCategoryID = (product.ProductSubCategoryID == null) ? 0 : product.ProductSubCategoryID;
            List<Product> products = new List<Product>();
            if (product.ProductSubCategoryID == 0 && product.Name == "" && product.ItemCode == "")
            {
                products = (List<Product>)await Set.Include(u => u.ProductSubCategory).OrderBy(j => j.Name).ToListAsync();
            }
            else if (product.ProductSubCategoryID == 0)
            {
                products = (List<Product>)await Set.Include(u => u.ProductSubCategory).Where(h => h.Name.Contains(product.Name) && h.ItemCode.Contains(product.ItemCode)).OrderBy(j => j.Name).ToListAsync();
            }
            else
            {
                products = (List<Product>)await Set.Include(u => u.ProductSubCategory).Where(h => h.Name.Contains(product.Name) && h.ItemCode.Contains(product.ItemCode) && h.ProductSubCategoryID == product.ProductSubCategoryID).OrderBy(j => j.Name).ToListAsync();
            }

            return await Task.FromResult<object>(products);
        }

        public async Task<int> UpdateAsync(int ProductID,object obj)
        {
            int result = 0;
            using (var transaction = _context.Database.BeginTransaction())
            {
                Product ToUpdate = await _context.Set<Product>().FirstOrDefaultAsync(u => u.ID == ProductID);
                Product prod = obj as Product;
                if (prod != null)
                {                   
                    ToUpdate.TaxRate =1;
                    ToUpdate.ProductSubCategoryID = prod.ProductSubCategoryID;
                    ToUpdate.RowVersion = prod.RowVersion;
                    ToUpdate.ProductCategoryID = prod.ProductCategoryID;
                    ToUpdate.BarCode = prod.BarCode;
                    ToUpdate.Name = prod.Name;
                    ToUpdate.ItemCode = prod.ItemCode;
                    ToUpdate.ReOrderLevel = prod.ReOrderLevel;
                    ToUpdate.TaxInclude = prod.TaxInclude;
                    ToUpdate.TaxGroupID = (prod.TaxGroupID==0) ? null : prod.TaxGroupID;
                    ToUpdate.ProductImage = (prod.ProductImage == null) ? ToUpdate.ProductImage : prod.ProductImage;

                    _context.MarkAsModified(ToUpdate);
                    result = await _context.SaveChangesAsync();
                }
                transaction.Commit();
            }

            return await Task.FromResult<int>(result);
        }

        public async Task<object> GetProductsByCategoryAsync(int subCategoryID)
        { 
            List<Product> products = new List<Product>();
            
            return await Task.FromResult<object>(await Set.Include(h => h.ProductSubCategory).Include(y => y.Inventories).ThenInclude(g => g.UnitChart).ThenInclude(y => y.UnitType).Where(f => f.ProductSubCategoryID == subCategoryID).ToListAsync());

            // return await Task.FromResult<object>(products);
        }

        public async Task<object> GetAllProductsAsync()
        {
           return await Task.FromResult<List<Product>>(await Set.OrderBy(h=>h.Name).ToListAsync());
        }

        public async Task<object> InsertProductAsync(object obj)
        {
            int result = 0;
            using var transaction = _context.Database.BeginTransaction();
            Product product = obj as Product;
            if (product != null && product.UnitCharts != null)
            {
                foreach (var item in product.UnitCharts)
                {
                    item.CreatedDate = DateTime.Now;
                    item.ModifiedDate = DateTime.Now;
                    await _context.UnitCharts.AddAsync(item);
                }
            }
            await _context.Products.AddAsync(product);
            result = await _context.SaveChangesAsync();
            transaction.Commit();

            return await Task.FromResult<object>(result);
        }
    }
}
