using EShopModels.Common;

namespace EShopModels.Services
{
    public interface IProductCategoryService : IService<ProductCategory>
    {
        Task<object> SelectAllAsync();
        Task<object> SearchAsync(object obj);
        Task UpdateProductCategoryAsync(ProductCategory productCategory);
        Task<object> GetAllSearchBoxItemsAsync();
        Task<object> GetSearchITemDetailAsync(int id);
    }
}
