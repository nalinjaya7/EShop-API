using EShopModels;
using EShopModels.Common;
using EShopModels.Repository;
using EShopModels.Services;
using System.Linq.Expressions;

namespace EShopServices
{
    public class BaseService<TEntity> : IService<TEntity> where TEntity : BaseEntity
    {
        public virtual IUnitOfWork _unitOfWork { get; private set; }
        private readonly IRepository<TEntity> _repository;
        private bool _disposed;

        public BaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = unitOfWork.Repository<TEntity>();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _repository.GetAll();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception)
            {
                return await _repository.GetAllAsync();
            }           
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _repository.FindAsync(predicate);
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _repository.Find(predicate);
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            return _repository.Get(filter, orderBy, includeProperties);
        }

        public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            return await _repository.GetAsync(filter, orderBy, includeProperties);
        }

        public TEntity GetSingle(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            return _repository.GetSingle(filter, orderBy, includeProperties);
        }

        public async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            return await _repository.GetSingleAsync(filter, orderBy, includeProperties);
        }

        public TEntity Add(TEntity entity)
        {
            _repository.Add(entity);
            _unitOfWork.Complete();
            return entity;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            _repository.Add(entity);
            await _unitOfWork.CompleteAsync();
            return entity;
        }

        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            _repository.AddRange(entities);
            _unitOfWork.Complete();
            return entities;
        }

        public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            _repository.AddRange(entities);
            await _unitOfWork.CompleteAsync();
            return entities;
        }

        public TEntity Remove(object id)
        {
            var entity = _repository.Remove(id);
            _unitOfWork.Complete();
            return entity;
        }

        public async Task<TEntity> RemoveAsync(object id)
        {
            var entity =await _repository.RemoveAsync(id); 
            return entity;
        }

        public IEnumerable<TEntity> RemoveRange(IEnumerable<TEntity> entity)
        {
            _repository.RemoveRange(entity);
            _unitOfWork.Complete();
            return entity;
        }

        public async Task<IEnumerable<TEntity>> RemoveRangeAsync(IEnumerable<TEntity> entity)
        {
            _repository.RemoveRange(entity);
            await _unitOfWork.CompleteAsync();
            return entity;
        }

        public TEntity Update(TEntity entity)
        {
            _repository.Update(entity);
            _unitOfWork.Complete();
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _repository.Update(entity);
            await _unitOfWork.CompleteAsync();
            return entity;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _unitOfWork.Dispose();
            }
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
