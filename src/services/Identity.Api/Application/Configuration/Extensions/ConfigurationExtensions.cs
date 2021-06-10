using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Api.Application.Configuration.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection SetupConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<AuthenticationConfig>(configuration.GetSection("Authentication"));
            services.Configure<DbConfig>(configuration.GetSection("Data"));
            
            return services;
        }
    }
}