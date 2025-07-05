using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Models;
using MyWebApi.Repository.IRepository;

namespace MyWebApi.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<Category> _categorydb;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
            _categorydb = _db.Set<Category>();
        }

        public Task UpdateAsync(Category category)
        {
            _categorydb.Update(category);
            return Task.CompletedTask;

        }
    }
}
