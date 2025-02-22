using EShopModels;
using EShopModels.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EShopRepository.Repositories
{
    public class UnitTypeRepository : BaseRepository<UnitType>, IUnitTypeRepository
    {
        public UnitTypeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<UnitType> InsertUnitTypeAsync(UnitType unitType)
        {
            await _context.Database.ExecuteSqlRawAsync("dbo.InsertUnitType @Code,@Name,@IsBaseUnit,@CreatedDate,@ModifiedDate,@IsDeleted",
            new SqlParameter("@Code", unitType.Code),
            new SqlParameter("@Name", unitType.Name),
            new SqlParameter("@IsBaseUnit", unitType.IsBaseUnit), 
            new SqlParameter("@CreatedDate", unitType.CreatedDate),
            new SqlParameter("@ModifiedDate", unitType.ModifiedDate),
            new SqlParameter("@IsDeleted", unitType.IsDeleted));
            return await Task.FromResult<UnitType>(unitType);
        }

        public async Task<object> SearchAsync(object obj)
        {
           return await Task.FromResult<List<UnitType>>(await Set.OrderBy(j => j.Name).ToListAsync<UnitType>()); 
        }

        public async Task<UnitType> UpdateUnitTypeAsync(UnitType unitType)
        {
            UnitType unitTypeToEdit = await _context.UnitTypes.Where(r => r.ID == unitType.ID).FirstOrDefaultAsync();
            unitTypeToEdit.ModifiedDate = DateTime.Now;
            unitTypeToEdit.IsBaseUnit = unitType.IsBaseUnit;
            unitTypeToEdit.IsDeleted = unitType.IsDeleted;
            unitTypeToEdit.RowVersion = unitType.RowVersion;
            unitTypeToEdit.Code = unitType.Code;
            unitTypeToEdit.Name = unitType.Name;
            _context.MarkAsModified(unitTypeToEdit);
            await _context.SaveChangesAsync();
            return await Task.FromResult<UnitType>(unitTypeToEdit);
         }
    }
}
