using EventManagement.Api.Core.Infrastructure.Extensions;
using EventManagement.Api.Core.Infrastructure.Output;
using EventManagement.Api.Core.Installers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagement.Web.Installers.Mvc
{
    public class MvcInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvc(opts => opts.AddCoreFilters())
                .AddCustomJsonOptions()
                .SetCompatibilityVersion(CompatibilityVersion.Latest);
        }
    }
}