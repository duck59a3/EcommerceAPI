using Hangfire;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MyWebApi.Data;
using MyWebApi.DI;
using MyWebApi.Helpers;
using MyWebApi.Middleware;
using MyWebApi.Providers;
using MyWebApi.Providers.IProviders;
using MyWebApi.Repository;
using MyWebApi.Repository.IRepository;
using MyWebApi.Services;
using MyWebApi.Services.IService;
using Serilog;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy => policy.WithOrigins("http://localhost:5173") // vite
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfire(x =>
    x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "MyWebAPI";
});
builder.Services.AddMemoryCache();
builder.Services.AddJWTAuthentication(builder.Configuration);
builder.Services.Configure<CloudinaryOptions>(
    builder.Configuration.GetSection("Cloudinary"));
// Configure file upload limits
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50MB
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddScoped<ICODPaymentProvider, CODPaymentProvider>();
builder.Services.AddScoped<IStripeProvider, StripeProvider>();

builder.Services.AddMyServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option => {
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "MyWebAPI", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.OperationFilter<FileUploadOperationFilter>();
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});



builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});
    
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowReact");
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseFileUploadMiddleware(50 * 1024 * 1024); // 50MB limit
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseMiddleware<GlobalException>();
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard(); // Truy c?p t?i /hangfire

app.MapControllers();
app.UseSerilogRequestLogging();
app.Run();
