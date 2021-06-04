using System.Collections.Generic;
using System.Reflection;
using Autofac;

namespace EventManagement.Web.Infrastructure.DI.Modules
{
    public class SettingsModule : Autofac.Module
    {
        private readonly IEnumerable<Assembly> _assemblies;

        public SettingsModule(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        protected override void Load(ContainerBuilder builder)
        {
            foreach (var assembly in _assemblies)
            {
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.EndsWith("Settings"))
                    .AsSelf()
                    .SingleInstance();
            }
        }
    }
}
