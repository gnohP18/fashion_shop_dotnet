using fashion_shop.API.Seeders;
using fashion_shop.Core.DTOs.Common;
using fashion_shop.Core.Entities;
using fashion_shop.Infrastructure.Database;
using fashion_shop.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SampleDotNet.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.IgnoreReadOnlyFields = true; });
;

//------------------------------ Service & Repo & Infra ----------------------------------//
services.AddInfrastructure(builder.Configuration);
services.AddServices();
services.AddRepositories();

//------------------------------ Mapper ------------------------------------------//
services.AddAutoMapper(typeof(Program));
services.AddHttpContextAccessor();

//------------------------------ Identity & Token Settings ----------------------------------//
services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));
services.AddIdentity<User, Role>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 4;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//------------------------------ CLI & Seeder ----------------------------------//
var app = builder.Build();

if (args.Contains("--seed-user"))
{
    using var scope = app.Services.CreateScope();
    await UserRoleSeeder.Seed(scope);

    return;
}

if (args.Contains("--seed-product"))
{
    using var scope = app.Services.CreateScope();
    await ProductCategorySeeder.Seed(scope);

    return;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRegisterMiddleware();
app.MapControllers();
app.Run();

