using Journal.Infrastructure.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace Journal.Infrastructure
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
        public static void Main(string[] args)
        {
            try
            {
                //TODO: Serilog
                //Log.Information("Configuring Journal.Infrastructure host ({ApplicationContext})...", AppName);
                Console.WriteLine("Configuring Journal.Infrastructure host ({ApplicationContext})...", AppName);

                IHost host = CreateHostBuilder(args);

                //Log.Information("Applying migrations ({ApplicationContext})...", AppName);
                Console.WriteLine("Applying migrations ({ApplicationContext})...", AppName);
                host.MigrateDbContext<SubjectContext>((context, services) =>
                {
                    var settings = services.GetService<IOptions<JournalInfrastructureSettings>>();
                    var logger = services.GetService<ILogger<SubjectContextSeed>>();

                    new SubjectContextSeed()
                        .SeedAsync(context, settings, logger)
                        .Wait();
                });
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
                Console.WriteLine("Program terminated unexpectedly ({ApplicationContext})!", ex.Message);
            }
            finally
            {
                //Log.CloseAndFlush();
            }


            Console.WriteLine("Hello World!");
        }


        public static IHost CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(hostConfig =>
                {
                    hostConfig.AddEnvironmentVariables(prefix: "DOTNET_");
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    Console.WriteLine($"host.HostingEnvironment.EnvironmentName : {hostContext.HostingEnvironment.EnvironmentName}");
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration settings = hostContext.Configuration;
                    services.Configure<JournalInfrastructureSettings>(settings);
                })
                .Build();
    }
}
