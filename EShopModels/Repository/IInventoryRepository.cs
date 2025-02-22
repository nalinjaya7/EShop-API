namespace EShopModels.Repository
{
    public interface IInventoryRepository : IRepository<Inventory>
    {
        Task<object> SearchAsync(object obj);
        Task<object> GetInventoriesForSearchAsync(Inventory inventory);
    }
}
