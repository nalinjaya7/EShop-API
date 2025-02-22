namespace EShopModels.Repository
{
    public interface IUnitTypeRepository : IRepository<UnitType>
    {
        Task<object> SearchAsync(object obj);
        Task<UnitType> InsertUnitTypeAsync(UnitType unitType);
        Task<UnitType> UpdateUnitTypeAsync(UnitType unitType);
    }
}
