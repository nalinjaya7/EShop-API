namespace EShopModels.Services
{
    public interface IUnitChartService : IService<UnitChart>
    {
        Task<object> SearchAsync(object obj);
        Task<object> GetUnitChartsByProductIDAsync(object obj);
        Task<object> UpdateUnitChartsAsync(int ProductID, object chartsList);
    }
}
