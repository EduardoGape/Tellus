using Microsoft.Extensions.DependencyInjection;
using TellusAPI.Application.Interfaces;
using TellusAPI.Application.Services;

namespace TellusAPI.Application.Configurations
{
    public static class ServiceRegistration
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IHelloWorldService, HelloWorldService>();
        }
    }
}

