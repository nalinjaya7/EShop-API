using EShopModels;
using EShopModels.Repository;
using EShopModels.Services;

namespace EShopServices.Services
{
    public class InventoryService : BaseService<Inventory>, IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _inventoryRepository = unitOfWork.InventoryRepository;
        }

        public Task<object> GetInventoriesForSearchAsync(Inventory inventory)
        {
            return _inventoryRepository.GetInventoriesForSearchAsync(inventory);
        }

        public async Task<object> SearchAsync(object obj)
        {
            return await _inventoryRepository.SearchAsync(obj);
        }
    } 
}
