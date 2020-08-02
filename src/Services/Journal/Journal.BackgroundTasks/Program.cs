using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Journal.BackgroundTasks
{
    public class Program
    {
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
                    services.AddHostedService<Worker>();

                    services.Configure<EventBusConfiguration>(eventBusConfig);

                    services.AddEventBus(eventBusConfig);
                });
    }
}
