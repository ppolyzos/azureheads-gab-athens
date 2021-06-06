using EventManagement.Api.Core.Installers;
using EventManagement.Web.Infrastructure.Extensions;
using EventManagement.Web.Infrastructure.Output;
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