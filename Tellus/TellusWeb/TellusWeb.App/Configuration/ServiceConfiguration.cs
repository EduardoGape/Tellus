using Microsoft.Extensions.DependencyInjection;
using TellusWeb.Application.Interfaces;
using TellusWeb.Application.Services;

namespace TellusWeb.App.Configuration
{
    public static class ServiceConfiguration
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped(sp =>
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri("http://localhost:5296/")
                };
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                return client;
            });

            services.AddScoped<IFunctionService, TellusWeb.Application.Services.FunctionService>();
            services.AddScoped<IProfileService, TellusWeb.Application.Services.ProfileService>();
            services.AddScoped<IUserService, TellusWeb.Application.Services.UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITokenDecoderService, TokenDecoderService>();
            services.AddScoped<IAuthorizationService, AuthorizationService>();
        }
    }
}