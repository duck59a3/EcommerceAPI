using MyWebApi.Services;
using MyWebApi.Services.IService;

namespace MyWebApi.DI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMyServices(this IServiceCollection services) {
            services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<IAppUser, UserService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}
