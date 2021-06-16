using System.IO;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace EventManagement.Web
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;

        public static readonly string AppName =
            Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static void Main(string[] args) =>
            CreateHostBuilder(args)
                .Build()
                .Run();

        private static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((host, builder) =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", optional: true,
                            reloadOnChange: true)
                        
                        .AddEnvironmentVariables();

                    if (host.HostingEnvironment.IsDevelopment())
                    {
                        builder.AddUserSecrets<Program>();
                    }
                })
                .UseSerilog((host, builder) =>
                {
                    var seqServerUrl = host.Configuration["Serilog:SeqServerUrl"];
                    var logstashUrl = host.Configuration["Serilog:LogstashUrl"];
                    var isDevelopment = host.HostingEnvironment.IsDevelopment();

                    builder.MinimumLevel.Verbose()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                        .Enrich.WithProperty("ApplicationContext", host.HostingEnvironment.ApplicationName)
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .WriteTo.Conditional(_ => isDevelopment, wt => wt.Seq(string.IsNullOrWhiteSpace(seqServerUrl)
                            ? "http://seq"
                            : seqServerUrl))
                        .WriteTo.Conditional(_ => isDevelopment, wt => wt.Http(string.IsNullOrWhiteSpace(logstashUrl)
                            ? "http://logstash:8080"
                            : logstashUrl))
                        .WriteTo.Conditional(_ => !isDevelopment, wt => wt.ApplicationInsights(TelemetryConverter.Traces))
                        .ReadFrom.Configuration(host.Configuration);
                })
                .UseStartup<Startup>()
                .CaptureStartupErrors(false)
                .UseContentRoot(Directory.GetCurrentDirectory());
    }
}