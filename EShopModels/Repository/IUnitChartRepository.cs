
namespace EShopModels.Repository
{
    public interface IUnitChartRepository : IRepository<UnitChart>
    {
        Task<object> SearchAsync(object obj);
        Task<object> GetUnitChartsByProductIDAsync(object obj);
        Task<object> UpdateUnitChartsAsync(int ProductID, object chartsList);
    }
}
