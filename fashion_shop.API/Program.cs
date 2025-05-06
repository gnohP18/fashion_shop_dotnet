using System.Text.Json;
using System.Text.Json.Serialization;
using fashion_shop.API.Seeders;
using fashion_shop.Core.DTOs.Common;
using fashion_shop.Core.Entities;
using fashion_shop.Infrastructure.Database;
using fashion_shop.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SampleDotNet.Api.Middlewares;
using static fashion_shop.API.Attributes.AuthenticateAttribute;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.IgnoreReadOnlyFields = true;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    }
);

//------------------------------ Firebase ----------------------------------//
services.Configure<FirebaseSettings>(builder.Configuration.GetSection("FirebaseDatabase"));

//------------------------------ Service & Repo & Infra ----------------------------------//
services.AddScoped<IAuthenticationFilterService, AuthenticationFilterService>();
services.AddInfrastructure(builder.Configuration);
services.AddServices();
services.AddRepositories();

//------------------------------ Mapper ------------------------------------------//
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddHttpContextAccessor();

//------------------------------ Minio Settings ----------------------------------//
services.Configure<MinioSettings>(builder.Configuration.GetSection("MinioSettings"));

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

//------------------------------ CORS ----------------------------------//
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

//------------------------------ CLI & Seeder ----------------------------------//
var app = builder.Build();
app.UseCors("AllowAll");
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

if (args.Contains("--seed-setting"))
{
    using var scope = app.Services.CreateScope();
    await SettingSeeder.Seed(scope);

    return;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseRegisterMiddleware();
app.MapControllers();
app.Run();

