using EShopModels;
using EShopModels.Common;
using EShopModels.Repository;
using EShopRepository.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace EShopRepository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private bool disposed;
        private Hashtable repositories; 
        private IEShopUserRepository _EShopUserRepository;
        private IProductCategoryRepository _productCategoryRepository;
        private IProductRepository _productRepository; 
        private IProductSubCategoryRepository _productSubCategoryRepository;
        private IUnitTypeRepository _unitTypeRepository;
        private IUnitChartRepository _unitChartRepository; 
        private IInventoryRepository _inventoryRepository; 
        private IShoppingCartRepository _shoppingCartRepository;
        private IShoppingCartItemRepository _shoppingCartItemRepository;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _applicationDbContext = dbContext;
        }

        public virtual IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            if (repositories == null)
            {
                repositories = new Hashtable();
            }
            var type = typeof(TEntity).Name;
            if (repositories.ContainsKey(type))
            {
                return (IRepository<TEntity>)repositories[type];
            }
            var repositorytype = typeof(BaseRepository<>);
            repositories.Add(type, Activator.CreateInstance(repositorytype.MakeGenericType(typeof(TEntity)), _applicationDbContext));
            return (IRepository<TEntity>)repositories[type];
        }
      

        public IInventoryRepository InventoryRepository
        {
            get
            {
                if (_inventoryRepository == null)
                {
                    _inventoryRepository = new InventoryRepository(_applicationDbContext);
                }
                return _inventoryRepository;
            }
        }

   
        public IUnitChartRepository UnitChartRepository
        {
            get
            {
                if (_unitChartRepository == null)
                {
                    _unitChartRepository = new UnitChartRepository(_applicationDbContext);
                }
                return _unitChartRepository;
            }
        }

        public IUnitTypeRepository UnitTypeRepository
        {
            get
            {
                if (_unitTypeRepository == null)
                {
                    _unitTypeRepository = new UnitTypeRepository(_applicationDbContext);
                }
                return _unitTypeRepository;
            }
        }

        public IProductSubCategoryRepository ProductSubCategoryRepository
        {
            get
            {
                if (_productSubCategoryRepository == null)
                {
                    _productSubCategoryRepository = new ProductSubCategoryRepository(_applicationDbContext);
                }
                return _productSubCategoryRepository;
            }
        }
         
        public IProductRepository ProductRepository
        {
            get
            {
                if (_productRepository == null)
                {
                    _productRepository = new ProductRepository(_applicationDbContext);
                }
                return _productRepository;
            }
        }

        public IProductCategoryRepository ProductCategoryRepository
        {
            get
            {
                if (_productCategoryRepository == null)
                {
                    _productCategoryRepository = new ProductCategoryRepository(_applicationDbContext);
                }
                return _productCategoryRepository;
            }
        }

        public IEShopUserRepository EShopUserRepository
        {
            get
            {
                if (_EShopUserRepository == null)
                {
                    _EShopUserRepository = new EShopUserRepository(_applicationDbContext);
                }
                return _EShopUserRepository;
            }
        } 
   
        public IShoppingCartRepository ShoppingCartRepository
        {
            get
            {
                if (_shoppingCartRepository == null)
                {
                    _shoppingCartRepository = new ShoppingCartRepository(_applicationDbContext);
                }
                return _shoppingCartRepository;
            }
        }

        public IShoppingCartItemRepository ShoppingCartItemRepository
        {
            get
            {
                if (_shoppingCartItemRepository == null)
                {
                    _shoppingCartItemRepository = new ShoppingCartItemRepository(_applicationDbContext);
                }
                return _shoppingCartItemRepository;
            }
        }
 
        public async Task<int> CompleteAsync()
        {
            return await _applicationDbContext.SaveChangesAsync();
        }
 
        public int Complete()
        {
            return _applicationDbContext.SaveChanges();
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                _applicationDbContext.Dispose();
            }
            this.disposed = true;
            GC.SuppressFinalize(this);
        }

        public async Task<int> InsertExecuteStoredProc(string StoreProcedureName,System.Collections.Generic.Dictionary<string, object> Parameters) 
        {
            int result = 0;
            DbConnection dbConnection = _applicationDbContext.Database.GetDbConnection();
            try
            {
                if(dbConnection.State != System.Data.ConnectionState.Open)
                {
                    await dbConnection.OpenAsync();
                }
                await using (DbCommand command = dbConnection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = StoreProcedureName;
                    foreach (KeyValuePair<string,object> parameter in Parameters)
                    {
                        DbParameter dbParameter = command.CreateParameter();
                        dbParameter.Value = parameter.Value;
                        dbParameter.ParameterName = parameter.Key;
                        command.Parameters.Add(dbParameter);
                    }
                    result = await command.ExecuteNonQueryAsync();
                }              
            }
            catch (Exception ex)
            {            
                await dbConnection.CloseAsync();
                await Task.FromException<Exception>(ex);
            }
            return await Task.FromResult<int>(result);
        }

        public async Task<List<T>> ExecuteStoredProc<T>(string storedProcName, Dictionary<string, object> procParams) where T : BaseEntity
        {
            DbConnection conn = _applicationDbContext.Database.GetDbConnection();
            try
            {
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                await using (DbCommand command = conn.CreateCommand())
                {
                    command.CommandText = storedProcName;
                    command.CommandType = CommandType.StoredProcedure;

                    foreach (KeyValuePair<string, object> procParam in procParams)
                    {
                        DbParameter param = command.CreateParameter();
                        param.ParameterName = procParam.Key;
                        param.Value = procParam.Value;
                        command.Parameters.Add(param);
                    }

                    DbDataReader reader = await command.ExecuteReaderAsync();
                    List<T> objList = new List<T>();
                    IEnumerable<PropertyInfo> props = typeof(T).GetRuntimeProperties();
                    Dictionary<string, DbColumn> colMapping = reader.GetColumnSchema()
                        .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
                        .ToDictionary(key => key.ColumnName.ToLower());

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            T obj = Activator.CreateInstance<T>();
                            foreach (PropertyInfo prop in props)
                            {
                                object val =
                                    reader.GetValue(colMapping[prop.Name.ToLower()].ColumnOrdinal.Value);
                                prop.SetValue(obj, val == DBNull.Value ? null : val);
                            }
                            objList.Add(obj);
                        }
                    }
                    reader.Dispose();

                    return objList;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message, e.InnerException);
            }
            finally
            {
                conn.Close();
            }

            return null; // default state
        }
    }
}
