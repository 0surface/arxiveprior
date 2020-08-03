using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.IO;

namespace Journal.Infrastructure
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
        public static readonly string EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();

            try
            {
                var host = CreateHostBuilder(args).Build();

                host.MigrateDbContext<SubjectContext>((context, services) =>
                {
                    var settings = services.GetService<IOptions<JournalConfiguration>>();
                    var logger = services.GetService<ILogger<SubjectContextSeed>>();

                    new SubjectContextSeed()
                        .SeedAsync(context, settings, logger)
                        .Wait();
                });

                host.Run();

                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
            finally
            {

            }
        }

        // EF Core uses this method at design time to access the DbContext
        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                        .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                        .ConfigureAppConfiguration((host, builder) =>
                        {
                            builder.SetBasePath(Directory.GetCurrentDirectory());
                            builder.AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                            builder.AddEnvironmentVariables();
                            builder.AddCommandLine(args);
                        })
                        .ConfigureLogging((host, builder) => builder.UseSerilog(host.Configuration).AddSerilog());


        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{EnvironmentName}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }       
    }
}
