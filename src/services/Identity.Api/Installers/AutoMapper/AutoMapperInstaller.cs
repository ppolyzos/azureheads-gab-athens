using System.Linq;
using System.Reflection;
using AutoMapper;
using EventManagement.Api.Core.Installers;
using EventManagement.Api.Core.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Identity.Api.Installers.AutoMapper
{
    public class AutoMapperInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            // var dependencyContext = DependencyContext.Default;
            // var assembliesToScan = dependencyContext.RuntimeLibraries
            //     .SelectMany(lib => lib.GetDefaultAssemblyNames(dependencyContext).Select(Assembly.Load));

            var assembliesToScan = PlatformUtils.GetAllAssemblies("Identity.Api");
            
            assembliesToScan = assembliesToScan as Assembly[] ?? assembliesToScan.ToArray();

            var allTypes = assembliesToScan.SelectMany(a => a.ExportedTypes).ToArray();

            var profiles = allTypes
                .Where(t => typeof(Profile).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo())
                            && !t.GetTypeInfo().IsAbstract)
                .ToArray();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullDestinationValues = true;

                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            });

            services.AddSingleton<IConfigurationProvider>(config);
            services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));
        }
    }
}
