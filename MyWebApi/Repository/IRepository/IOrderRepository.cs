using MyWebApi.Models;

namespace MyWebApi.Repository.IRepository
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task UpdateAsync(Order entity);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        

    }
}
