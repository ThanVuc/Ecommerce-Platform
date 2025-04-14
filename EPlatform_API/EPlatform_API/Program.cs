using System.Net.NetworkInformation;
using EPlatform_API.Data;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.Helper;
using EPlatform_API.IRepository;
using EPlatform_API.IServices;
using EPlatform_API.Repository;
using EPlatform_API.Services;
using EPlatform_API.UnitOfWork;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Explicitly configure Kestrel to listen on port 5120 for HTTPS
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5120, listenOptions => listenOptions.UseHttps()); // Bind to all interfaces()); // Bind to localhost
});

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

// Configure HTTPS redirection
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 5120;  // Port for HTTPS
});

// ConfigureApplicationCookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.Cookie.Name = ".AspNetCore.Identity.Application";
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

// Config Hangfire
services.ConfigureHangfire(configuration);

// DI
services.AddTransient<IUnitOfWork, UnitOfWork>();
services.AddTransient<ITokenService, TokenService>();
services.AddTransient<IPasswordHasher, PasswordHasher>();
services.AddSingleton<ISendMailService, SendMailService>();
services.AddScoped<ISeedDataService, SeedDataService>();
services.AddScoped<IQueryingServices, QueryingServices>();
services.AddScoped<IRedisServices, RedisServices>();
services.AddSingleton<ILoggingService, LoggingService>();
services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
services.AddScoped<IVietnameseLocationRepository, VietnameseLocationRepository>();
services.AddScoped<ProductInfoMongoRepository, ProductInfoMongoRepository>();
services.AddScoped<UserRepo, UserRepo>();
services.AddScoped<OrderRepository, OrderRepository>();
services.AddScoped<ShopRepository, ShopRepository>();
services.AddScoped<NotificationRepo, NotificationRepo>();
services.AddScoped<SearchMongoRepo, SearchMongoRepo>();
services.AddSingleton<ISynchronizationService, SynchronizationService>();
services.AddScoped<RedisServices, RedisServices>();
services.AddSingleton<IRecurringJobManager, RecurringJobManager>();


var app = builder.Build();
app.UseHttpsRedirection();  
// Configure the HTTP request pipeline.
// apply migrations to the database
using (var scope = app.Services.CreateAsyncScope()){
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.MigrateAsync();
    var vietnamContext = scope.ServiceProvider.GetRequiredService<VietnameseLocationContext>();
    await vietnamContext.Database.MigrateAsync();
}


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAllCORS");

app.UseAuthentication();

app.UseAuthorization();

// Ensure Hangfire is initialized before using its APIs
app.UseHangfireDashboard(); // Retain only the dashboard middleware

using (var scope = app.Services.CreateScope())
{
    var recurringJobs = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    recurringJobs.AddOrUpdate<ISynchronizationService>(
        "UpdateAutocompleteData",
        service => service.UpdateAutocompleteData(),
        Cron.Daily
    );
}

app.MapHub<NotificationHub>("notificationHub");

app.MapControllers();

app.Run();
