using System;
using EventManagement.Api.Core.Installers;
using Identity.Api.Application.Configuration;
using Identity.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Identity.Api.Installers.Db
{
    public class InMemoryDbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var sp = services.BuildServiceProvider();
            var dbConfig = sp.GetService<IOptions<DbConfig>>()?.Value;
            if (dbConfig != null && dbConfig.Database != DbType.InMemory) return;

            const string name = "Catalog";
            services.AddDbContext<AuthDbContext>(c => c.UseInMemoryDatabase(name));

            Activator.CreateInstance<EfCoreIdentityInstaller>().InstallServices(services, configuration);
        }
    }
}