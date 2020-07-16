using arx.Extract.BackgroundTasks.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace arx.Extract.BackgroundTasks
{
    public class Program
    {
        public IConfiguration Configuration { get; }
        public static readonly string AppName = typeof(Program).Assembly.GetName().Name;
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Run();
        }

        public static IHost CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)            
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddHostedService<ScheduledArchiveService>();
                    services.Configure<BackgroundTaskSettings>(hostContext.Configuration.GetSection("settings"));
                    Console.WriteLine($"host.HostingEnvironment.EnvironmentName : {hostContext.HostingEnvironment.EnvironmentName}");
                })
            //TODO : Configure SeriLog
            .Build();
    }
}
