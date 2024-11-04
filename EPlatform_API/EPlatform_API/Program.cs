using EPlatform_API.Data;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IServices;
using EPlatform_API.Services;
using EPlatform_API.UnitOfWork;
using Microsoft.OpenApi.Models;

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
services.ConfigRedisCatching();

// Route Config
services.ConfigureRoute();

// Config JWT
services.ConfigureJWT(configuration);

// Config fix load API with NewtonSoftJson
services.ConfigureAPILoad();

// Cors Config
services.ConfigureCORS();

// DI
services.AddTransient<IUnitOfWork,UnitOfWork>();
services.AddTransient<ITokenService,TokenService>();
services.AddTransient<IPasswordHasher,PasswordHasher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAllCORS");

app.UseAuthorization();

app.MapControllers();

app.Run();
