namespace EShopModels.Repository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<object> GetProductInventoriesAsync(int ProductID); 
        Task<object> SearchAsync(object obj);
        Task<object> SearchAsync(object obj, Product SearchText);
        Task<object> GetByIDAsync(int id); 
        Task<int> UpdateAsync(int ProductID,object obj);
        Task<object> GetProductsByCategoryAsync(int subCategoryID);
        Task<object> GetAllProductsAsync();
        Task<object> InsertProductAsync(object product);
    }
}
