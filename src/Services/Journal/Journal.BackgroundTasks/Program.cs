using Autofac.Extensions.DependencyInjection;
using Journal.BackgroundTasks.Config;
using Journal.BackgroundTasks.Services;
using Journal.BackgroundTasks.Tasks;
using Journal.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Journal.BackgroundTasks
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
                    string connectionString = hostContext.Configuration["ConnectionString"];

                    services.AddHostedService<ArchiveJournalProcessingService>();
                    services.AddConfiguredGrpcClient();

                    services.Configure<EventBusConfiguration>(eventBusConfig)
                            .Configure<JournalBackgroundTasksConfiguration>(hostContext.Configuration)
                            .Configure<UrlsConfig>(hostContext.Configuration.GetSection("urls"));

                    services.AddEventBus(eventBusConfig)
                            .AddConfiguredAutoMapper()
                            .AddScoped<IFulfillmentRepository, FulfillmentRepository>()                            
                            .AddScoped<ExtractGrpcService, ExtractGrpcService>()
                            .AddScoped<ITransformService, TransformService>()
                            .AddCustomDbContext(connectionString);
                })
            .ConfigureLogging((host, builder) => builder.UseSerilog(host.Configuration).AddSerilog())
            .Build();
    }
}
