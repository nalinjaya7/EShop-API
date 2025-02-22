using EShopModels;
using EShopModels.Repository;
using EShopModels.Services;

namespace EShopServices.Services
{
    public class UnitChartService : BaseService<UnitChart>,IUnitChartService
    {
        private readonly IUnitChartRepository _unitChartRepository;
        public UnitChartService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitChartRepository = unitOfWork.UnitChartRepository;
        }

        public async Task<object> GetUnitChartsByProductIDAsync(object obj)
        {
            return await _unitChartRepository.GetUnitChartsByProductIDAsync(obj);
        }

        public async Task<object> SearchAsync(object obj)
        {
            return await _unitChartRepository.SearchAsync(obj);
        }

        public async Task<object> UpdateUnitChartsAsync(int ProductID, object chartsList)
        {
            return await _unitChartRepository.UpdateUnitChartsAsync(ProductID, chartsList);
        }
    } 
}
