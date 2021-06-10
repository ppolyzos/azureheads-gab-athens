using Identity.Api.Data;
using Identity.Api.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Api.Installers.Db
{
    public class EfCoreIdentityInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(o =>
                {
                    o.SignIn.RequireConfirmedEmail = false; 
                    o.SignIn.RequireConfirmedPhoneNumber = false;
                    o.Tokens.ChangePhoneNumberTokenProvider = "Phone";
                })
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();
        }
    }
}