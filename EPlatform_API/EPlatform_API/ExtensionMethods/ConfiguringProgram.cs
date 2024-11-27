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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace EPlatform_API.ExtensionMethods
{
    public static class ConfiguringProgram
    {
        public static void AddSqlDBContext(this IServiceCollection services, IConfiguration configuration){
            services.AddDbContext<AppDbContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("Default"));
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
            string signingKey = configuration["JWT:SigningKey"];
            if (signingKey.IsNullOrEmpty()){
                throw new Exception("The Signing Key of your system hasn't set yet!");
            }
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme =
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters(){
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
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
                    builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders("X-Pagination");
                });
            });
        }

        public static void ConfigRedisCatching(this IServiceCollection services){
            services.AddStackExchangeRedisCache(options => {
                options.Configuration = "localhost";
                options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions(){
                    AbortOnConnectFail = true,
                    EndPoints = {options.Configuration}
                };
            });
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
    }

    
}