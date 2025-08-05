
using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Repository.IRepository;
using System.Linq.Expressions;

namespace MyWebApi.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> _dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
            _db.Products.Include(u => u.Category).Include(u => u.CategoryId);
        }

        public async Task AddAsync(T entity)
        {
           await _dbSet.AddAsync(entity);

        }

        public Task RemoveAsync(T entity)
        {
             _dbSet.Remove(entity);
            return Task.CompletedTask;


        }

        public async Task<T> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        //public T Get(Expression<Func<T, bool>> filter, bool tracked = false, string? includeProperties = null)
        //{
        //    IQueryable<T> query;
        //    if (tracked)
        //    {
        //        query = dbSet;

        //    }
        //    else
        //    {
        //        query = dbSet.AsNoTracking();

        //    }
        //    query = query.Where(filter);
        //    if (!string.IsNullOrEmpty(includeProperties))
        //    {
        //        foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        //        {
        //            query = query.Include(includeProperty);
        //        }
        //    }
        //    return query.FirstOrDefault();
        //}
    }
}
