using Autofac.Extensions.DependencyInjection;
using Journal.BackgroundTasks.Tasks;
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
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration eventBusConfig = hostContext.Configuration.GetSection("EventBus");

                    services.AddHostedService<ArchiveJournalProcessingService>();

                    services.Configure<EventBusConfiguration>(eventBusConfig)
                            .Configure<JournalBackgroundTasksConfiguration>(hostContext.Configuration);

                    services.AddEventBus(eventBusConfig);
                })
            .ConfigureLogging((host, builder) => builder.UseSerilog(host.Configuration).AddSerilog());
    }
}
