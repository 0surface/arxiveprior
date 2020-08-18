using arx.Extract.BackgroundTasks.Extensions;
using arx.Extract.BackgroundTasks.Tasks;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

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
                    IConfiguration eventBusConfig = hostContext.Configuration.GetSection("EventBus");
                    IConfiguration storageConfig = hostContext.Configuration.GetSection("Storage");

                    services.AddHostedService<ArchiveExtractionService>();

                    services.Configure<BackgroundTasksConfiguration>(hostContext.Configuration)
                            .Configure<EventBusConfiguration>(eventBusConfig)
                            .Configure<StorageConfiguration>(storageConfig);

                    services.AddEventBus(eventBusConfig)
                            .AddAutoMapper(typeof(Program))
                            .AddSubjectRepository(storageConfig)
                            .AddJobRepository(storageConfig)
                            .AddJobItemRepository(storageConfig)
                            .AddFulfillmentRepository(storageConfig)
                            .AddFulfillmentItemRepository(storageConfig)
                            .AddPublicationRepository(storageConfig)
                            .AddExtractService()
                            .AddArchiveFetch()
                            .AddTransformService();
                })
            .ConfigureLogging((host, builder) => builder.UseSerilog(host.Configuration).AddSerilog())
            .Build();
    }
}
