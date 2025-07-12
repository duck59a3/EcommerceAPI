using MyWebApi.Models;

namespace MyWebApi.Repository.IRepository
{
    public interface IProductImageRepository : IRepository<ProductImage>
    {
        Task UpdateAsync(ProductImage productImage);
    }
}
