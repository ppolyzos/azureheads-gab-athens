using EventManagement.Api.Core.Installers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Api.Installers.Auth
{
    public class AuthPasswordPolicyInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<IdentityOptions>(o =>
            {
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 5;

                o.SignIn.RequireConfirmedEmail = false; // check as well in DbServiceExtensions to disable it also there
                o.SignIn.RequireConfirmedPhoneNumber = false;
            });
        }
    }
}