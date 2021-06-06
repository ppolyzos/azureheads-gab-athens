﻿using EventManagement.Api.Core.Installers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagement.Web.Installers.Tools
{
    public class ApplicationInsightsInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationInsightsTelemetry();
        }
    }
}