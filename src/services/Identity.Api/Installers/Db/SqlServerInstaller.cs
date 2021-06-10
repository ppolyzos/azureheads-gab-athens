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
    public class SqlServerInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var sp = services.BuildServiceProvider();
            var dbConfig = sp.GetService<IOptions<DbConfig>>()?.Value;
            if (dbConfig != null && dbConfig.Database != DbType.SqlServer) return;


            services.AddDbContextPool<AuthDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("IdentityDataContextConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(5,
                            TimeSpan.FromSeconds(10),
                            null);
                    }
                ));

            Activator.CreateInstance<EfCoreIdentityInstaller>().InstallServices(services, configuration);
        }
    }
}