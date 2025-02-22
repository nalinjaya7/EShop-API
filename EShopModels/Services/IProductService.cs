namespace EShopModels.Services
{
    public interface IProductService : IService<Product>
    {
        Task<object> GetProductInventoriesAsync(int ProductID); 
        Task<object> SearchAsync(object obj);
        Task<object> SearchAsync(object obj, Product SearchText);
        Task<object> GetByIDAsync(int id); 
        Task<int> UpdateAsync(int ProductID, object obj);
        Task<object> GetProductsByCategoryAsync(int subCategoryID);
        Task<object> GetAllProductsAsync(); 
    }
}
