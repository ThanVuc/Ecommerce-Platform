using System.Net.NetworkInformation;
using EPlatform_API.Data;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.Helper;
using EPlatform_API.IRepository;
using EPlatform_API.IServices;
using EPlatform_API.Repository;
using EPlatform_API.Services;
using EPlatform_API.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
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


// Entity Framework
services.AddSqlDBContext(configuration);

// Config Redis Caching
services.ConfigRedisCatching(configuration);

// Config MongoDB
services.ConfigureMongoDB(configuration);

// Route Config
services.ConfigureRoute();

// Config Identity Framework
services.ConfigureIdentityFramework();

// Config JWT
services.ConfigureJWT(configuration);

// Config fix load API with NewtonSoftJson
services.ConfigureAPILoad();

// Cors Config
services.ConfigureCORS();

// Policy
services.ConfigurePolicy();

// Config SignalR
services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
});

// DI
services.AddTransient<IUnitOfWork,UnitOfWork>();
services.AddTransient<ITokenService,TokenService>();
services.AddTransient<IPasswordHasher,PasswordHasher>();
services.AddSingleton<ISendMailService,SendMailService>();
services.AddScoped<ISeedDataService,SeedDataService>();
services.AddScoped<IQueryingServices,QueryingServices>();
services.AddScoped<IRedisServices, RedisServices>();
services.AddSingleton<ILoggingService, LoggingService>();
services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
services.AddScoped<IVietnameseLocationRepository,VietnameseLocationRepository>();
services.AddScoped<ProductInfoMongoRepository,ProductInfoMongoRepository>();
services.AddScoped<UserRepo,UserRepo>();
services.AddScoped<OrderRepository,OrderRepository>();
services.AddScoped<ShopRepository,ShopRepository>();
services.AddScoped<NotificationRepo,NotificationRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAllCORS");

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<NotificationHub>("notificationHub");

app.MapControllers();

app.Run();
