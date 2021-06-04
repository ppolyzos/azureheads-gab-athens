using System.Collections.Generic;
using System.Reflection;
using Autofac;

namespace EventManagement.Web.Infrastructure.DI.Modules
{
    public class RepositoryModule : Autofac.Module
    {
        private readonly IEnumerable<Assembly> _assemblies;

        public RepositoryModule(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        protected override void Load(ContainerBuilder builder)
        {
            foreach (var assembly in _assemblies)
            {
                // Query Instantiation
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.EndsWith("Query"))
                    .AsSelf();

                // Repository Instantiation
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.EndsWith("Repository"))
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
            }
        }
    }
}
