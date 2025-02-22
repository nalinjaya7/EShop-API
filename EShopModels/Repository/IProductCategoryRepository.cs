using EShopModels.Common;

namespace EShopModels.Repository
{
    public interface IProductCategoryRepository : IRepository<ProductCategory>
    {
        Task<object> SelectAllAsync();
        Task<object> SearchAsync(object obj);
        Task<int> UpdateAsync(int ID, object obj);
        Task<object> GetAllSearchBoxItemsAsync();
        Task<object> GetSearchITemDetailAsync(int id);
    }
}
