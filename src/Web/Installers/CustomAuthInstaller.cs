using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EventManagement.Api.Core.Installers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EventManagement.Web.Installers
{
    public class CustomAuthInstaller : IInstaller
    {
        private static readonly string[] HubEndpoints = { "echo" };
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            configuration = configuration.GetSection("Authentication");
            var symmetricKeyAsBase64 = configuration["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);

            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = configuration["Issuer"],

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = configuration["Audience"],

                // Validate the token expiry
                ValidateLifetime = true,

                ClockSkew = TimeSpan.FromMinutes(1) // 1 minute tolerance for the expiration date
            };

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = true;
                    o.SaveToken = true;
                    o.TokenValidationParameters = tokenValidationParameters;
                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = HandleMessageReceived,
                        OnAuthenticationFailed = HandleAuthenticationFailed,
                    };
                });
        }
        
        private static Task HandleAuthenticationFailed(AuthenticationFailedContext context)
        {
            context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;

            return Task.FromResult(context);
        }

        private static Task HandleMessageReceived(MessageReceivedContext context)
        {
            var path = context.Request.Path.Value;
            if (HubEndpoints.Any(hubEndpoint => path != null && path.StartsWith($"/{hubEndpoint}"))
                && context.Request.Query.TryGetValue("token", out var token))
            {
                context.Token = token;
            }

            return Task.CompletedTask;
        }
    }
}