using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagement.Web.Configuration.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection SetupConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<SessionizeConfig>(configuration.GetSection("Sessionize"));

            return services;
        }
    }
}