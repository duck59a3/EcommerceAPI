using MyWebApi.Services.IService;

namespace MyWebApi.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IRepository<T> Repository<T>() where T : class;
        IUserRepository Users { get; }
        Task SaveAsync();
    }
}
