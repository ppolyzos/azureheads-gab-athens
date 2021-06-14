using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using Autofac;
using Identity.Api.Data.Models;
using Identity.Api.Data.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Infrastructure.DI
{
    public class StandardModule : Autofac.Module
    {
        private readonly IEnumerable<Assembly> _assemblies;

        public StandardModule(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }
        
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PasswordHasher<ApplicationUser>>().As<IPasswordHasher<ApplicationUser>>()
                .InstancePerLifetimeScope();
            builder.RegisterType<CurrentUser>().As<ICurrentUser>();
            builder.RegisterType<HttpClient>().AsSelf().InstancePerLifetimeScope();
        }
    }
}