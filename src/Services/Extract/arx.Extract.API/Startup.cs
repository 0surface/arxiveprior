using arx.Extract.API.Services;
using arx.Extract.Data.Repository;
using AutoMapper;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace arx.Extract.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            IConfiguration storageConfig = Configuration.GetSection("Storage");
            services.AddControllers();
            services.Configure<StorageConfiguration>(storageConfig)
                    .AddCustomSwagger()
                    .AddGrpc()
                    .AddHealthChecks(storageConfig)
                    .AddConfiguredAutoMapper()
                    .AddJournalService(storageConfig)
                    .AddFulfillmentService(storageConfig)
                    .AddFulfillmentItemService(storageConfig);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger<Startup>().LogDebug("Using PATH BASE '{pathBase}'", pathBase);
                app.UsePathBase(pathBase);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCustomSwagger(pathBase);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<JournalService>();
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }
    }
    static class CustomExtensionMethods
    {
        #region Service Collection

        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            hcBuilder.AddAzureTableStorage(
                configuration["StorageConnectionString"],
                tableName: configuration["JobTableName"],
                name: "ExtractDB-check",
                 tags: new string[] { "extractdb" });

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "arxiveprior - Extract HTTP API",
                    Version = "v1",
                    Description = "The arx Extract Service HTTP API"
                });
            });
            return services;
        }

        public static IServiceCollection AddJournalService(this IServiceCollection services, IConfiguration configuration)
        {
            var storageConnectionString = configuration["StorageConnectionString"];
            var publicationTableName = configuration["PublicationTableName"];
            var fulfillmentTableName = configuration["FulfillmentTableName"];

            services.AddScoped<JournalService>(opt =>
            {
                return new JournalService(ConfigureAutoMapper(),
                    new PublicationRepository(storageConnectionString, publicationTableName),
                    new FulfillmentRepository(storageConnectionString, fulfillmentTableName));
            });
            return services;
        }

        public static IServiceCollection AddFulfillmentService(this IServiceCollection services, IConfiguration configuration)
        {
            var storageConnectionString = configuration["StorageConnectionString"];
            var tableName = configuration["FulfillmentTableName"];

            services.AddScoped<IFulfillmentService>(opt =>
            {
                return new FulfillmentService(new FulfillmentRepository(storageConnectionString, tableName), ConfigureAutoMapper());
            });
            return services;
        }

        public static IServiceCollection AddFulfillmentItemService(this IServiceCollection services, IConfiguration configuration)
        {
            var storageConnectionString = configuration["StorageConnectionString"];
            var tableName = configuration["FulfillmentItemTableName"];

            services.AddScoped<IFulfillmentItemService>(opt =>
            {
                return new FulfillmentItemService(new FulfillmentItemRepository(storageConnectionString, tableName), ConfigureAutoMapper());
            });
            return services;
        }

        public static IServiceCollection AddConfiguredAutoMapper(this IServiceCollection services)
        {
            services.AddSingleton(ConfigureAutoMapper());
            return services;
        }

        public static IMapper ConfigureAutoMapper()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ApiAutoMapperProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            return mapper;
        }

        public static IServiceCollection AddGrpc(this IServiceCollection services)
        {
            return services.AddGrpc(opt =>
            {
                opt.EnableDetailedErrors = true;
            }).Services;
        }

        #endregion Service Collection

        #region Middleware

        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app, string pathBase)
        {
            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "Extract.API V1");
                    c.OAuthClientId("extractswaggerui");
                    c.OAuthAppName("Extract Swagger UI");
                });

            return app;
        }

        #endregion Middleware
    }
}
