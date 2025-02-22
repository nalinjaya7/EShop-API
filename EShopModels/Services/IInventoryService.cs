namespace EShopModels.Services
{
    public interface IInventoryService : IService<Inventory>
    {
        Task<object> SearchAsync(object obj);
        Task<object> GetInventoriesForSearchAsync(Inventory inventory);
    }
}
