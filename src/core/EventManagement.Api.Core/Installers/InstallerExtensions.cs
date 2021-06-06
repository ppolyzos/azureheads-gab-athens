using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagement.Api.Core.Installers
{
    public static class InstallerExtensions
    {
        public static void InstallServicesInAssembly<T>(this IServiceCollection services, IConfiguration configuration)
        {
            var installers = typeof(T).Assembly.ExportedTypes
                .Where(c => typeof(IInstaller).IsAssignableFrom(c)
                            && !c.IsInterface
                            && !c.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IInstaller>()
                .ToList();
            
            installers.ForEach(installer => installer.InstallServices(services, configuration));
        }
    }
}