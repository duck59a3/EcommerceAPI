using MyWebApi.Services.IService;

namespace MyWebApi.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IRepository<T> Repository<T>() where T : class;
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        IUserRepository Users { get; }
        ICartRepository Carts { get; }
        IOrderRepository Orders { get; }
        IPaymentRepository Payments { get; }
        IProductImageRepository ProductImages { get; }
        IReviewRepository Reviews { get; }
        IVoucherRepository Vouchers { get; }
        IVoucherUsageRepository VouchersUsage { get; }
        Task SaveAsync();
    }
}
