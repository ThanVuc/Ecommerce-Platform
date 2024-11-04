using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EPlatform_API.Data;
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
                    .AllowAnyMethod();
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
    }

    
}