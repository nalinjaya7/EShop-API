using EShopModels.Common;
using EShopModels.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EShopRepository
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        public ApplicationDbContext _context;
        private DbSet<TEntity> _set;

        public BaseRepository()
        {

        }

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        protected DbSet<TEntity> Set
        {
            get
            {
                return _set ??= _context.Set<TEntity>();
            }
        }

        public virtual TEntity Add(TEntity obj)
        {
            return Set.Add(obj).Entity;
        }

        public virtual TEntity Remove(object id)
        {
            TEntity? entity = Set.Find(id);
            Set.Attach(entity);
            return Set.Remove(entity).Entity;
        }

        public virtual async Task<TEntity> RemoveAsync(object id)
        {
            TEntity entity =await Set.FindAsync(id);
            Set.Attach(entity);
            return Set.Remove(entity).Entity;
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Set.ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Set.ToListAsync();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.Where(predicate).ToList();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Set.Where(predicate).ToListAsync();
        }

        public TEntity Get(int ID)
        {
            return Set.Find(ID);
        }

        public async Task<TEntity> GetAsync(int ID)
        {
            return await Set.FindAsync(ID);
        }

        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            Set.AddRange(entities);
            return entities;
        }

        public IEnumerable<TEntity> RemoveRange(IEnumerable<TEntity> entity)
        {
            Set.RemoveRange(entity);
            return entity;
        }

        public virtual TEntity Update(TEntity entity)
        {
            entity = Set.Attach(entity).Entity;
            _context.MarkAsModified(entity);
            return entity;
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = Set;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = Set;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public TEntity GetSingle(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = Set;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).FirstOrDefault();
            }
            else
            {
                return query.FirstOrDefault();
            }
        }

        public virtual async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = Set;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).FirstOrDefaultAsync();
            }
            else
            {
                return await query.FirstOrDefaultAsync();
            }
        }
    }
}
