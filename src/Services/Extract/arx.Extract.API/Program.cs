using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace arx.Extract.API
{
    public class Program
    {
        public static readonly string EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        public static void Main(string[] args)
        {
            var configuration = GetConfiguration();
            CreateHostBuilder(configuration, args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    //webBuilder.ConfigureKestrel(options =>
                    //{
                    //    var ports = GetDefinedPorts(configuration);
                    //    options.Listen(IPAddress.Any, ports.httpPort, listenOptions =>
                    //    {
                    //        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                    //    });

                    //    options.Listen(IPAddress.Any, ports.grpcPort, listenOptions =>
                    //    {
                    //        listenOptions.Protocols = HttpProtocols.Http2;
                    //    });
                    //});
                });


        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{EnvironmentName}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        private static (int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
        {
            var grpcPort = config.GetValue("GRPC_PORT", 81);
            var port = config.GetValue("PORT", 80);
            return (port, grpcPort);
        }
    }
}
