using System;
using System.Threading.Tasks;
using Autofac;
using Identity.Api.Application.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Identity.Api.Data.Seed.Core
{
    public class AuthDbContextDataSeeder
    {
        private readonly IWebHostEnvironment _env;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly DbConfig _config;
        private readonly ILoggerFactory _logger;
        private readonly IContainer _container;

        public AuthDbContextDataSeeder(IServiceScopeFactory scopeFactory,
            IOptions<DbConfig> config,
            IWebHostEnvironment env,
            ILoggerFactory logger,
            IContainer container)
        {
            _scopeFactory = scopeFactory;
            _config = config.Value;
            _env = env;
            _logger = logger;
            _container = container;
        }

        public async Task SeedAsync(int? retry = 0)
        {
            if (!_env.IsDevelopment() || !this._config.Seed) return;

            var retryForAvailability = retry.GetValueOrDefault(0);
            var log = _logger.CreateLogger("application seed");

            try
            {
                using var scope = _scopeFactory.CreateScope();
                await _container.Resolve<UserDataSeeder>().SeedAsync();
            }
            catch (Exception ex)
            {
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;
                    log.LogError(ex.Message);
                    await SeedAsync(retryForAvailability);
                }
            }
        }
    }
}