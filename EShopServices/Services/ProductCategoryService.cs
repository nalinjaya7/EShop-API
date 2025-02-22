using EShopModels;
using EShopModels.Common;
using EShopModels.Repository;
using EShopModels.Services;

namespace EShopServices.Services
{
    public class ProductCategoryService : BaseService<ProductCategory>,IProductCategoryService
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        public ProductCategoryService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _productCategoryRepository = unitOfWork.ProductCategoryRepository;
        }

        public async Task<object> GetAllSearchBoxItemsAsync()
        {
            return await _productCategoryRepository.GetAllSearchBoxItemsAsync();
        }

        public async Task<object> GetSearchITemDetailAsync(int id)
        {
            return await _productCategoryRepository.GetSearchITemDetailAsync(id);
        }

        public async Task<object> SearchAsync(object obj)
        {           
            return await _productCategoryRepository.SearchAsync(obj);
        }

        public async Task<object> SelectAllAsync()
        {           
            return await _productCategoryRepository.SelectAllAsync();
        }
 
        public Task UpdateProductCategoryAsync(ProductCategory productCategory)
        {
            throw new System.NotImplementedException();
        }
    } 
}
