using EShopModels;
using EShopModels.Repository;
using EShopModels.Services;

namespace EShopServices.Services
{
    public class ProductService : BaseService<Product>, IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _productRepository = unitOfWork.ProductRepository;
        }

        public async Task<object> GetByIDAsync(int id)
        {           
            return await _productRepository.GetByIDAsync(id);
        }
 
        public async Task<object> GetProductInventoriesAsync(int ProductID)
        {
            return await _productRepository.GetProductInventoriesAsync(ProductID);
        }

        public async Task<object> SearchAsync(object obj)
        {
            return await _productRepository.SearchAsync(obj);
        }

        public async Task<object> SearchAsync(object obj, Product product)
        {
            return await _productRepository.SearchAsync(obj, product);
        }

        public async Task<int> UpdateAsync(int ProductID, object obj)
        {
            return await _productRepository.UpdateAsync(ProductID, obj);
        }
 
        public async Task<object> GetProductsByCategoryAsync(int subCategoryID)
        {
            return await _productRepository.GetProductsByCategoryAsync(subCategoryID);
        }

        public async Task<object> GetAllProductsAsync()
        {
            return await _productRepository.GetAllProductsAsync();
        }
 
    } 
}
