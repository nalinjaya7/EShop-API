namespace EShopModels.Services
{
    public interface IUnitTypeService : IService<UnitType>
    {
        Task<object> SearchAsync(object obj);
        Task<UnitType> InsertUnitTypeAsync(UnitType unitType);
        Task<UnitType> UpdateUnitTypeAsync(UnitType unitType);
    }
}
