using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Api.Application.Configuration.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection SetupConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration);
            services.Configure<AuthenticationConfig>(configuration.GetSection("Authentication"));
            services.Configure<DbConfig>(configuration.GetSection("Data"));
            services.Configure<EmailConfig>(configuration.GetSection("Email"));
            services.Configure<SmsConfig>(configuration.GetSection("Sms"));
            
            return services;
        }
    }
}