using EPlatform_API.Data;
using EPlatform_API.ExtensionMethods;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Entity Framework
services.AddSqlDBContext(configuration);

// Route Config
services.ConfigureRoute();

// Config JWT
services.ConfigureJWT(configuration);

// Config fix load API with NewtonSoftJson


// Cors Config
services.ConfigureCORS();

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
