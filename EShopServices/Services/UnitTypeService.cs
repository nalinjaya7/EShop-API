using EShopModels;
using EShopModels.Repository;
using EShopModels.Services;

namespace EShopServices.Services
{
    public class UnitTypeService : BaseService<UnitType>,IUnitTypeService
    {
        private readonly IUnitTypeRepository _unitTypeRepository;
        public UnitTypeService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitTypeRepository = unitOfWork.UnitTypeRepository;
        }

        public async Task<UnitType> InsertUnitTypeAsync(UnitType unitType)
        {
            return await _unitTypeRepository.InsertUnitTypeAsync(unitType);
        }

        public async Task<object> SearchAsync(object obj)
        {
            return await _unitTypeRepository.SearchAsync(obj);
        }

        public async Task<UnitType> UpdateUnitTypeAsync(UnitType unitType)
        {
            return await _unitTypeRepository.UpdateUnitTypeAsync(unitType);
        }
    } 
}
