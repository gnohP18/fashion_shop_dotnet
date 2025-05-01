using fashion_shop.Core.DTOs.Common;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.Infrastructure.Database;
using fashion_shop.Infrastructure.Repositories;
using fashion_shop.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using StackExchange.Redis;

namespace fashion_shop.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductItemService, ProductItemService>();
            services.AddScoped<IAdminAuthService, AdminAuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IMediaFileService, MediaFileService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IStatisticService, StatisticService>();
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<IPersonalProfileService, PersonalProfileService>();

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IMediaFileRepository, MediaFileRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
            services.AddScoped<IProductItemRepository, ProductItemRepository>();
            services.AddScoped<IVariantRepository, VariantRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options => options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention());

            var redisConnectionString = configuration.GetConnectionString("RedisConnection");
            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
            }

            services.AddSingleton(resolver =>
            {
                var config = resolver.GetRequiredService<IOptions<MinioSettings>>().Value;
                return new MinioClient()
                    .WithEndpoint(config.Endpoint)
                    .WithCredentials(config.AccessKey, config.SecretKey)
                    .WithSSL(false)
                    .Build();
            });

            return services;
        }
    }
}