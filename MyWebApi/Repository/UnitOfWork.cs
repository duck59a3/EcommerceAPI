using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Repository.IRepository;
using MyWebApi.Services.IService;

namespace MyWebApi.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private readonly Dictionary<Type, object> _repositories = new();
        public IUserRepository Users { get; private set; }
        public ICartRepository Carts { get; private set; }
        //public ICategoryRepository Category { get; private set; }
        //public IProductRepository Product { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            //Category = new CategoryRepository(_db);
            //Product = new ProductRepository(_db);
            Users = new UserRepository(_db);
            Carts = new CartRepository(_db);
        }

       

        public IRepository<T> Repository<T>() where T : class
        {
            if (!_repositories.ContainsKey(typeof(T)))
            {
                var repoInstance = new Repository<T>(_db);
                _repositories.Add(typeof(T), repoInstance);
            }

            return (IRepository<T>)_repositories[typeof(T)];
        }


        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
    
}
