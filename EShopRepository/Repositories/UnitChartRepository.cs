using EShopModels;
using EShopModels.Repository;
using Microsoft.EntityFrameworkCore;

namespace EShopRepository.Repositories
{
    public class UnitChartRepository : BaseRepository<UnitChart>, IUnitChartRepository
    {
        public UnitChartRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<object> GetUnitChartsByProductIDAsync(object obj)
        {
            List<UnitChart> ucs = new();
            if (obj != null)
            {
                ucs = await Set.Include(K => K.UnitType).Where(t => t.ProductID == (int)obj && t.IsDeleted == false).ToListAsync();
            }
            return await Task.FromResult<object>(ucs);
        }

        public async Task<object> SearchAsync(object obj)
        {
          return await Task.FromResult<List<UnitChart>>(await Set.OrderBy(j => j.UnitTypeName).ToListAsync<UnitChart>()); 
        }

        public async Task<object> UpdateUnitChartsAsync(int ProductID, object chartsList)
        {
            List<UnitChart> resultunitCharts = new();
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    if (chartsList is List<UnitChart> unitCharts)
                    {
                        foreach (UnitChart item in unitCharts)
                        {
                            item.UnitType = null;
                            if (item.ID == 0)
                            {
                                _context.Set<UnitChart>().Add(item);
                            }
                            else
                            {
                                UnitChart unit = await Set.Where(d => d.ProductID == item.ProductID && d.UnitTypeID == item.UnitTypeID).FirstOrDefaultAsync();
                                unit.ModifiedDate = DateTime.Now;
                                unit.Quantity = item.Quantity;
                                unit.UnitChartName = item.UnitChartName;
                                unit.UnitTypeID = item.UnitTypeID;
                                unit.RowVersion = item.RowVersion;
                                _context.MarkAsModified(unit);
                            }
                        }

                        List<UnitChart> ucs = await Set.AsNoTracking().Where(d => d.ProductID == ProductID).ToListAsync();
                        foreach (UnitChart item in ucs)
                        {
                            var rchitem = unitCharts.SingleOrDefault(y => y.ID == item.ID);
                            if (rchitem == null)
                            {
                                item.ModifiedDate = DateTime.Now;
                                _context.MarkAsModified(item);
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }

                resultunitCharts = await Set.Include(K => K.UnitType).Where(d => d.ProductID == ProductID && d.IsDeleted == false).ToListAsync();
            }
            catch(Exception)
            {
                throw;
            }
            return await Task.FromResult<List<UnitChart>>(resultunitCharts);
        }
    }
}
