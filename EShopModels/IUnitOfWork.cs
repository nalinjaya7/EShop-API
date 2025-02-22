using EShopModels.Common;
using EShopModels.Repository;

namespace EShopModels
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;  
        IEShopUserRepository EShopUserRepository { get; }
        IProductCategoryRepository ProductCategoryRepository { get; }
        IProductRepository ProductRepository { get; } 
        IProductSubCategoryRepository ProductSubCategoryRepository { get; }
        IUnitTypeRepository UnitTypeRepository { get; }
        IUnitChartRepository UnitChartRepository { get; } 
        IInventoryRepository InventoryRepository { get; } 
        IShoppingCartRepository ShoppingCartRepository { get; }
        IShoppingCartItemRepository ShoppingCartItemRepository { get; }

        Task<int> InsertExecuteStoredProc(string StoreProcedureName,Dictionary<string,object> Parameters);
        Task<List<T>> ExecuteStoredProc<T>(string storedProcName, Dictionary<string, object> procParams) where T : BaseEntity;
        int Complete();
        Task<int> CompleteAsync();
    }
}
