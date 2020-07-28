using arx.Extract.BackgroundTasks.Extensions;
using arx.Extract.BackgroundTasks.Tasks;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
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
                    services.AddHostedService<ArchiveExtractionService>();
                    services.Configure<BackgroundTaskSettings>(settings);
                    services.AddEventBus(settings)
                            .AddAutoMapper(typeof(Program))
                            .AddSubjectRepository(settings)
                            .AddJobRepository(settings)
                            .AddJobItemRepository(settings)
                            .AddFulfillmentRepository(settings)
                            .AddFulfillmentItemRepository(settings)
                            .AddPublicationRepository(settings)
                            .AddExtractService()
                            .AddArchiveFetch()
                            .AddTransformService();
                            

                    Console.WriteLine($"host.HostingEnvironment.EnvironmentName : {hostContext.HostingEnvironment.EnvironmentName}");
                })
            //TODO : Configure SeriLog
            .Build();
    }
}
