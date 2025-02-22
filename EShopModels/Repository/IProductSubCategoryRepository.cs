namespace EShopModels.Repository
{
    public interface IProductSubCategoryRepository : IRepository<ProductSubCategory>
    {
        Task<object> SearchAsync(object obj);
        Task<object> SearchAsync();
        Task<object> GetSubCategoriesByCategoryAsync(int CategoryID, int pageNumber);
        Task<object> GetSubCategoriesByCategoryAsync(int categoryID);
        Task<object> GetSubCategoryByID(int id);
    }
}
