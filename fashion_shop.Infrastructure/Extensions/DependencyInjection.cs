using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.Infrastructure.Database;
using fashion_shop.Infrastructure.Repositories;
using fashion_shop.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace fashion_shop.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IAdminAuthService, AdminAuthService>();
            services.AddSingleton<ITokenService, TokenService>();

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options => options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention());

            var redisConnectionString = configuration.GetConnectionString("RedisConnection");

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}