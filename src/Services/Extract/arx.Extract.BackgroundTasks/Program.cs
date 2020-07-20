using arx.Extract.BackgroundTasks.Extensions;
using arx.Extract.BackgroundTasks.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace arx.Extract.BackgroundTasks
{
    public class Program
    {
        public static readonly string AppName = typeof(Program).Assembly.GetName().Name;
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Run();
        }

        public static IHost CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration settings = hostContext.Configuration.GetSection("settings");
                    services.AddHostedService<ScheduledArchiveService>();
                    services.Configure<BackgroundTaskSettings>(settings);
                    services.AddEventBus(settings)
                            .AddSubjectRepository(settings)
                            .AddJobRepository(settings)
                            .AddJobItemRepository(settings)
                            .AddFulfilmentRepository(settings)
                            .AddFulfilmentItemRepository(settings)
                            .AddPublicationRepository(settings);
                    
                    Console.WriteLine($"host.HostingEnvironment.EnvironmentName : {hostContext.HostingEnvironment.EnvironmentName}");
                })
            //TODO : Configure SeriLog
            .Build();
    }
}
