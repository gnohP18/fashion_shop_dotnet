using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Infrastructure.Database.Configurations.Entities;
using fashion_shop.Infrastructure.Database.EntityConfiguration;
using fashion_shop.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace fashion_shop.Infrastructure.Database;

public class ApplicationDbContext : IdentityDbContext<User, Role, int>, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<MediaFile> MediaFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().ToTable("users");
        builder.Entity<Role>().ToTable("roles");
        builder.Entity<IdentityUserRole<int>>().ToTable("user_roles");
        builder.Entity<IdentityUserClaim<int>>().ToTable("user_claims");
        builder.Entity<IdentityUserLogin<int>>().ToTable("user_logins");
        builder.Entity<IdentityRoleClaim<int>>().ToTable("role_claims");
        builder.Entity<IdentityUserToken<int>>().ToTable("user_tokens");

        // Apply configurations ...
        // builder.ApplyConfiguration(new ProductConfiguration());
        // builder.ApplyConfiguration(new CategoryConfiguration());

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

    }
}