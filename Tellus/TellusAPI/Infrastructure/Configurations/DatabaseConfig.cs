using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Data;
using Npgsql;

namespace TellusAPI.Infrastructure.Configurations
{
    public static class DatabaseConfig
    {
        public static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            services.AddScoped<IDbConnection>(provider => 
                new NpgsqlConnection(connectionString));
        }
    }
}