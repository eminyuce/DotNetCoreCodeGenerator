using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFGenericRepository
{
    /// <summary>
    /// IEntityRepository implementation for DbContext instance.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <typeparam name="TId">Type of entity Id</typeparam>
    public class EntityRepository<TEntity, TId> : IEntityRepository<TEntity, TId>
        where TEntity : class, IEntity<TId>
        where TId : IComparable
    {

        private readonly IEntitiesContext _dbContext;


        public EntityRepository(IEntitiesContext dbContext)
        {

            if (dbContext == null)
            {

                throw new ArgumentNullException("dbContext");
            }

            _dbContext = dbContext;
        }


        public IQueryable<TEntity> GetAll()
        {

            return _dbContext.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {

            IQueryable<TEntity> queryable = GetAll();
            foreach (Expression<Func<TEntity, object>> includeProperty in includeProperties)
            {

                queryable = queryable.Include<TEntity, object>(includeProperty);
            }

            return queryable;
        }

        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {

            IQueryable<TEntity> queryable = GetAll().Where<TEntity>(predicate);
            return queryable;
        }

        public PaginatedList<TEntity> Paginate(int pageIndex, int pageSize)
        {

            PaginatedList<TEntity> paginatedList = Paginate<TId>(pageIndex, pageSize, x => x.Id);
            return paginatedList;
        }

        public PaginatedList<TEntity> Paginate<TKey>(int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> keySelector)
        {

            return Paginate<TKey>(pageIndex, pageSize, keySelector, null);
        }

        public PaginatedList<TEntity> Paginate<TKey>(int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> keySelector, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {

            PaginatedList<TEntity> paginatedList = Paginate<TKey>(
                pageIndex, pageSize, keySelector, predicate, OrderByType.Ascending, includeProperties);

            return paginatedList;
        }

        public PaginatedList<TEntity> PaginateDescending<TKey>(int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> keySelector)
        {

            return PaginateDescending<TKey>(pageIndex, pageSize, keySelector, null);
        }

        public PaginatedList<TEntity> PaginateDescending<TKey>(int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> keySelector, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {

            PaginatedList<TEntity> paginatedList = Paginate<TKey>(
                pageIndex, pageSize, keySelector, predicate, OrderByType.Descending, includeProperties);

            return paginatedList;
        }

        public TEntity GetSingle(TId id)
        {

            IQueryable<TEntity> entities = GetAll();
            TEntity entity = Filter<TId>(entities, x => x.Id, id).FirstOrDefault();
            return entity;
        }

        public TEntity GetSingleIncluding(TId id, params Expression<Func<TEntity, object>>[] includeProperties)
        {

            IQueryable<TEntity> entities = GetAllIncluding(includeProperties);
            TEntity entity = Filter<TId>(entities, x => x.Id, id).FirstOrDefault();
            return entity;
        }

        public void Add(TEntity entity)
        {

            _dbContext.SetAsAdded(entity);
        }

        public void AddGraph(TEntity entity)
        {

            _dbContext.Set<TEntity>().Add(entity);
        }

        public void Update(TEntity t, object key)
        {
            if (t == null)
                return;
            TEntity exist = _dbContext.Set<TEntity>().Find(key);
            if (exist != null)
            {
                _dbContext.Entry(exist).CurrentValues.SetValues(t);
            }
        }

        public void Delete(TEntity entity)
        {
            _dbContext.SetAsDeleted(entity);
        }

        public int Save()
        {
            return _dbContext.SaveChanges();
        }

        public bool Contains(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Count<TEntity>(predicate) > 0;
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var objects = FindBy(predicate);
            foreach (var obj in objects)
                _dbContext.Set<TEntity>().Remove(obj);
        }

        public IQueryable<TEntity> FindAll<TKey>(Expression<Func<TEntity, bool>> match, Expression<Func<TEntity, TKey>> keySelector, OrderByType orderByType, int? take, int? skip)
        {
            var queryable = FindAllQueryable(match, keySelector, orderByType, take, skip);
            return queryable;
        }

        private IQueryable<TEntity> FindAllQueryable<TKey>(Expression<Func<TEntity, bool>> match, Expression<Func<TEntity, TKey>> keySelector, OrderByType orderByType, int? take,
                                                  int? skip)
        {
            var queryable = FindBy(match);
            queryable = (orderByType == OrderByType.Ascending)
                            ? queryable.OrderBy(keySelector)
                            : queryable.OrderByDescending(keySelector);
            if (skip.HasValue && skip.Value > 0)
            {
                queryable = queryable.Skip(skip.Value);
            }
            if (take.HasValue && take.Value > 0)
            {
                queryable = queryable.Take(take.Value);
            }
            return queryable;
        }




        public int Count()
        {
            return GetAll().Count();
        }

        public int Count(Expression<Func<TEntity, bool>> match)
        {
            return FindBy(match).Count();
        }

        public IQueryable<TEntity> FindAllIncluding<TKey>(Expression<Func<TEntity, bool>> match, Expression<Func<TEntity, TKey>> keySelector, OrderByType orderByType,
                                         int? take, int? skip, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var queryable = _dbContext.Set<TEntity>().Where(match);
            queryable = (orderByType == OrderByType.Ascending)
                            ? queryable.OrderBy(keySelector)
                            : queryable.OrderByDescending(keySelector);

            foreach (Expression<Func<TEntity, object>> includeProperty in includeProperties)
            {
                queryable = queryable.Include<TEntity, object>(includeProperty);
            }
            if (skip.HasValue && skip.Value > 0)
            {
                queryable = queryable.Skip(skip.Value);
            }
            if (take.HasValue && take.Value > 0)
            {
                queryable = queryable.Take(take.Value);
            }

            return queryable;
        }




        // Privates

        private PaginatedList<TEntity> Paginate<TKey>(int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> keySelector, Expression<Func<TEntity, bool>> predicate, OrderByType orderByType, params Expression<Func<TEntity, object>>[] includeProperties)
        {

            IQueryable<TEntity> queryable =
                (orderByType == OrderByType.Ascending)
                    ? GetAllIncluding(includeProperties).OrderBy(keySelector)
                    : GetAllIncluding(includeProperties).OrderByDescending(keySelector);

            queryable = (predicate != null) ? queryable.Where(predicate) : queryable;
            PaginatedList<TEntity> paginatedList = queryable.ToPaginatedList(pageIndex, pageSize);

            return paginatedList;
        }

        private IQueryable<TEntity> Filter<TProperty>(IQueryable<TEntity> dbSet,
            Expression<Func<TEntity, TProperty>> property, TProperty value)
            where TProperty : IComparable
        {

            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null || !(memberExpression.Member is PropertyInfo))
            {

                throw new ArgumentException("Property expected", "property");
            }

            Expression left = property.Body;
            Expression right = Expression.Constant(value, typeof(TProperty));
            Expression searchExpression = Expression.Equal(left, right);
            Expression<Func<TEntity, bool>> lambda = Expression.Lambda<Func<TEntity, bool>>(
                searchExpression, new ParameterExpression[] { property.Parameters.Single() });

            return dbSet.Where(lambda);
        }


        public virtual async Task<List<TEntity>> GetAllAsync()
        {

            return await _dbContext.Set<TEntity>().ToListAsync();
        }


        public virtual async Task<TEntity> GetAsync(int id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }


        public virtual async Task<TEntity> AddAsync(TEntity t)
        {
            _dbContext.Set<TEntity>().Add(t);
            await _dbContext.SaveChangesAsync();
            return t;

        }

        public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match)
        {
            return await _dbContext.Set<TEntity>().SingleOrDefaultAsync(match);
        }

        public async Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match)
        {
            return await _dbContext.Set<TEntity>().Where(match).ToListAsync();
        }

        public virtual async Task<int> DeleteAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            return await _dbContext.SaveChangesAsync();
        }


        public virtual async Task<TEntity> UpdateAsync(TEntity t, object key)
        {
            if (t == null)
                return null;
            TEntity exist = await _dbContext.Set<TEntity>().FindAsync(key);
            if (exist != null)
            {
                _dbContext.Entry(exist).CurrentValues.SetValues(t);
                await _dbContext.SaveChangesAsync();
            }
            return exist;
        }

        public async Task<int> CountAsync()
        {
            return await _dbContext.Set<TEntity>().CountAsync();
        }

        public async virtual Task<int> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }


        public virtual async Task<List<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbContext.Set<TEntity>().Where(predicate).ToListAsync();
        }



        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        public virtual async Task<TEntity> SaveOrUpdateAsync(TEntity entity, object key)
        {
            int entityKey = (int)key;
            if (entityKey == 0)
            {
                return await AddAsync(entity);
            }
            else
            {
                return await UpdateAsync(entity, key);
            }
        }
        public TEntity SaveOrUpdate(TEntity entity, object key)
        {
            int entityKey = (int)key;
            if (entityKey == 0)
            {
                Add(entity);
            }
            else
            {
                Update(entity, key);
            }
            this.Save();

            return entity;
        }
        public IEnumerable<TEntity> List(ISpecification<TEntity> spec)
        {
            // fetch a Queryable that includes all expression-based includes
            var queryableResultWithIncludes = spec.Includes
                .Aggregate(_dbContext.Set<TEntity>().AsQueryable(),
                    (current, include) => current.Include(include));

            // modify the IQueryable to include any string-based include statements
            var secondaryResult = spec.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));

            // return the result of the query using the specification's criteria expression
            return secondaryResult
                            .Where(spec.Criteria)
                            .AsEnumerable();
        }
        public async Task<List<TEntity>> ListAsync(ISpecification<TEntity> spec)
        {
            // fetch a Queryable that includes all expression-based includes
            var queryableResultWithIncludes = spec.Includes
                .Aggregate(_dbContext.Set<TEntity>().AsQueryable(),
                    (current, include) => current.Include(include));

            // modify the IQueryable to include any string-based include statements
            var secondaryResult = spec.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));

            // return the result of the query using the specification's criteria expression
            return await secondaryResult
                            .Where(spec.Criteria)
                            .ToListAsync();
        }

    }

}
