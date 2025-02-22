using EShopModels;
using EShopModels.Repository;
using EShopModels.Services;

namespace EShopServices.Services
{
    public class ProductSubCategoryService : BaseService<ProductSubCategory>,IProductSubCategoryService
    {
        private readonly IProductSubCategoryRepository _productSubCategoryRepository;
        public ProductSubCategoryService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _productSubCategoryRepository = unitOfWork.ProductSubCategoryRepository;
        }

        public async Task<object> GetSubCategoriesByCategoryAsync(int CategoryID, int pageNumber)
        {
            return await _productSubCategoryRepository.GetSubCategoriesByCategoryAsync(CategoryID, pageNumber);
        }

        public async Task<object> GetSubCategoriesByCategoryAsync(int categoryID)
        {
            return await _productSubCategoryRepository.GetSubCategoriesByCategoryAsync(categoryID);
        }

        public async Task<object> GetSubCategoryByID(int id)
        {
            return await _productSubCategoryRepository.GetSubCategoryByID(id);
        }

        public async Task<object> SearchAsync(object obj)
        {
            return await _productSubCategoryRepository.SearchAsync(obj);
        }

        public async Task<object> SearchAsync()
        {
            return await _productSubCategoryRepository.SearchAsync();
        }
    } 
}
