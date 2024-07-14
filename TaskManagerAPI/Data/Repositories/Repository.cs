using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManagerAPI.DTO.Response;

namespace TaskManagerAPI.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly TaskManagementDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(TaskManagementDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task InsertRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, string sortField, string sortOrder, int pageNumber, int pageSize)
        {
            var queryable = _dbSet.Where(predicate);

            if (!string.IsNullOrEmpty(sortField))
            {
                queryable = string.Equals(sortOrder, "Desc", StringComparison.OrdinalIgnoreCase)
                            ? queryable.OrderByDescending(GetSortExpression(sortField))
                            : queryable.OrderBy(GetSortExpression(sortField));
            }

            var totalItems = await queryable.CountAsync();
            var items = await queryable.Skip((pageNumber - 1) * pageSize)
                                       .Take(pageSize)
                                       .ToListAsync();

            return new PaginatedList<TEntity>(items, totalItems);
        }

        private static Expression<Func<TEntity, object>> GetSortExpression(string sortField)
        {
            var param = Expression.Parameter(typeof(TEntity), "x");
            var body = Expression.Convert(Expression.PropertyOrField(param, sortField), typeof(object));
            return Expression.Lambda<Func<TEntity, object>>(body, param);
        }
    }
}
