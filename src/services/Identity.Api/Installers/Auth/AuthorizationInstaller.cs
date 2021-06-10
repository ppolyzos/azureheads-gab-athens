using System;
using EventManagement.Api.Core.Installers;
using Identity.Api.Application.Policies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Api.Installers.Auth
{
    public class AuthorizationInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var instance = Activator.CreateInstance<AuthServerPolicies>();

            // if (!(Activator.CreateInstance<T>() is ICustomPolicyBuilder instance))
            //     throw new ArgumentException("Value of T should be of type ICustomPolicyBuilder");
            
            if (instance == null)
                throw new ArgumentException("Value of T should be of type ICustomPolicyBuilder");    
            
            services.AddAuthorization(o =>
            {
                foreach (var (key, value) in instance.GetPolicies())
                {
                    o.AddPolicy(key, value);
                }
            });
        }
    }
}