using MyWebApi.Models;

namespace MyWebApi.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task UpdateAsync(Product entity);
    }
}
