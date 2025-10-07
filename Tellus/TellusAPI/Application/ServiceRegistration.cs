using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;
using Dapper;
using TellusAPI.Application.Interfaces;
using TellusAPI.Application.Services;
using TellusAPI.Domain.Entities;
using TellusAPI.Domain.Entities.Reference;
using System.Collections.Generic;

namespace TellusAPI.Application.Configurations
{
    public static class ServiceRegistration
    {
        private static bool _handlersRegistered = false;

        public static void ConfigureServices(IServiceCollection services)
        {
            if (!_handlersRegistered)
            {
                SqlMapper.AddTypeHandler(new JsonTypeHandler<List<Function>>());
                SqlMapper.AddTypeHandler(new JsonTypeHandler<ProfileReference>());
                _handlersRegistered = true;
            }

            services.AddScoped<IDbConnection>(_ =>
                new NpgsqlConnection("Host=localhost;Database=tellus;Username=postgres;Password=postgres;Port=5432"));

            services.AddScoped<IDatabaseExecutor, DapperDatabaseExecutor>();
            services.AddScoped<IHelloWorldService, HelloWorldService>();
            services.AddScoped<IFunctionService, FunctionService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IUserService, UserService>();
        }
    }
}
