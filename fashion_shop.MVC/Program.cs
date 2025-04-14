using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using fashion_shop.Core.Entities;
using fashion_shop.Infrastructure.Database;
using fashion_shop.Infrastructure.Extensions;
using fashion_shop.Core.DTOs.Common;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.
// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// services.AddDbContext<ApplicationDbContext>(options => options
//                 .UseNpgsql(connectionString)
//                 .UseSnakeCaseNamingConvention());

//------------------------------ Service & Repo & Infra ----------------------------------//
services.AddInfrastructure(builder.Configuration);
services.AddServices();
services.AddRepositories();

//------------------------------ Mapper ------------------------------------------//
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddHttpContextAccessor();

//------------------------------ Minio Settings ----------------------------------//
services.Configure<MinioSettings>(builder.Configuration.GetSection("MinioSettings"));

services.AddDatabaseDeveloperPageExceptionFilter();
services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


services.AddControllersWithViews();
services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
