using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EPlatform_API.Data;
using EPlatform_API.Models;
using EPlatform_API.Setting;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using MongoDB.Driver;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EPlatform_API.ExtensionMethods
{
    public static class ConfiguringProgram
    {
        public static void AddSqlDBContext(this IServiceCollection services, IConfiguration configuration){
            services.AddDbContext<AppDbContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("Default"));
            });

            services.AddDbContext<VietnameseLocationContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("VietNamDB"));
            });
        }

        public static void ConfigureRoute(this IServiceCollection services){
            services.Configure<RouteOptions>(options => {
                options.AppendTrailingSlash = false;
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = false;
            });
        }

        public static void ConfigureIdentityFramework(this IServiceCollection services){
            
            services.AddOptions();

            services.AddIdentity<AppUser, IdentityRole>()   
            .AddEntityFrameworkStores<AppDbContext>();


            services.Configure<IdentityOptions>(options => {
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email là duy nhất

                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = false;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
                options.SignIn.RequireConfirmedAccount = false;
            });

        }
    
        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration){
            var jwtSettings = configuration.GetSection("JWT").Get<JwtSetting>();
            if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.SecretKey)){
                throw new Exception("The Signing Key of your system hasn't set yet!");
            }
            var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme =
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.HttpContext.Request.Cookies["access_token"];

                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        // ✅ Ensure the token is passed for SignalR requests
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };

            });
        }

        public static void ConfigureAPILoad(this IServiceCollection services){
            services.AddControllers().AddNewtonsoftJson(options => {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
        }

        public static void ConfigureCORS(this IServiceCollection services){
            services.AddCors(options => {
                options.AddPolicy("AllowAllCORS",
                builder => {
                    Console.WriteLine("Origin: " + builder.ToString());
                    builder
                    .SetIsOriginAllowed(origin => {
                            Console.WriteLine($"Origin: {origin}");
                            if (string.IsNullOrEmpty(origin))
                            {
                                return false;
                            }

                            try
                            {
                                return new Uri(origin).Host.EndsWith("eplatform.online");
                            }
                            catch
                            {
                                return false;
                            }
                    })
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithExposedHeaders("X-Pagination");
                });
            });
        }

        public static void ConfigRedisCatching(this IServiceCollection services, IConfiguration configuration){
            var redisConnectionString = configuration.GetConnectionString("Cloud_RedisDB");
            ConfigurationOptions redisConfig = new ConfigurationOptions{
                EndPoints = { {redisConnectionString, 19350} },
                User = "default",
                Password = configuration["Redis:Password"], // Replace with your Redis password if needed
                AbortOnConnectFail = false
            };
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig));
        }

        public static void ConfigurePolicy(this IServiceCollection services){
            services.AddAuthorization(options => {
                options.AddPolicy("RoleManagePolicy", policy => {
                    policy.RequireClaim("CanManipulateRolePage","true");
                });
                options.AddPolicy("UserManagePolicy", policy => {
                    policy.RequireClaim("CanManipulateUserPage","true");
                });
            });
        }
    
        public static void ConfigureMongoDB(this IServiceCollection services, IConfiguration configuration){
            services.AddSingleton<IMongoClient,MongoClient>(sp => {
                return new MongoClient(configuration.GetConnectionString("Cloud_MongoDB"));
            });

            services.AddSingleton(sp => {
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(configuration["MongoDB:Database"]);
            });
        }

        public static void ApplyMigrations(this IApplicationBuilder app){
            using (var scope = app.ApplicationServices.CreateScope()){
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var context_vietnam = scope.ServiceProvider.GetRequiredService<VietnameseLocationContext>();
                context.Database.Migrate();
                context_vietnam.Database.Migrate();
            }
        }

        public static void ConfigureHangfire(this IServiceCollection services, IConfiguration configuration){
            services.AddHangfire(config =>
            {
                MongoClient mongoClient = null;
                IMongoDatabase mongoDatabase = null;
                int retryCount = 5; // Number of retry attempts
                int delayMilliseconds = 2000; // Delay between retries

                for (int i = 0; i < retryCount; i++)
                {
                    try
                    {
                        mongoClient = new MongoClient(configuration.GetConnectionString("Cloud_MongoDB"));
                        mongoDatabase = mongoClient.GetDatabase(configuration["MongoDB:Database"]);
                        break; // Exit loop if connection is successful
                    }
                    catch (Exception ex)
                    {
                        if (i == retryCount - 1) // If last attempt fails, rethrow the exception
                        {
                            throw new Exception("Failed to connect to MongoDB after multiple attempts.", ex);
                        }
                        Task.Delay(delayMilliseconds).Wait(); // Wait before retrying
                    }
                }

                var mongoStorageOptions = new MongoStorageOptions
                {
                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new MigrateMongoMigrationStrategy(), // Automatically migrate the schema
                        BackupStrategy = new CollectionMongoBackupStrategy(), // Backup existing data before migration
                    },
                    CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection,
                    CheckConnection = false, // Disable initial database ping
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5), // Increase timeout for MongoDB operations
                };
                config.UseMongoStorage(mongoClient, mongoDatabase.DatabaseNamespace.DatabaseName, mongoStorageOptions);
                config.UseSimpleAssemblyNameTypeSerializer();
                config.UseRecommendedSerializerSettings();
            });

            services.AddHangfireServer(); // Move Hangfire server configuration here
        }

    }
}