using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace fashion_shop.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}