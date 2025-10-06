using TellusAPI.Application.Configurations;
using TellusAPI.Infrastructure.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// 1) Services primeiro
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2) CORS - Configuração específica para desenvolvimento
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(origin => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configurações customizadas
ServiceRegistration.ConfigureServices(builder.Services);
AuthenticationConfig.ConfigureAuthentication(builder.Services, builder.Configuration);
DatabaseConfig.ConfigureDatabase(builder.Services, builder.Configuration);
SwaggerConfig.ConfigureSwagger(builder.Services);

var app = builder.Build();

// 3) CORS primeiro
app.UseCors("AllowAll");

// 4) Swagger em Dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tellus API v1");
        c.RoutePrefix = "swagger";
    });

    app.MapGet("/", () => Results.Redirect("/swagger"));
}

// 5) Pipeline - REMOVA o HTTPS Redirection temporariamente
// app.UseHttpsRedirection(); // COMENTE ESTA LINHA

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();