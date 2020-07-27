using arx.Extract.API.Services;
using arx.Extract.Data.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            var serviceConfig = Configuration.GetSection("Settings");
            services.AddControllers();
            services.AddCustomSwagger()
                    .AddConfiguredAutoMapper()
                    .AddPublicationService(serviceConfig)
                    .AddFulfillmentService(serviceConfig);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var pathBase = Configuration["PATH_BASE"];

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCustomSwagger(pathBase);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
    static class CustomExtensionMethods
    {
        #region Service Collection
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "arx.Extract - Extrat HTTP API",
                    Version = "v1",
                    Description = "The arx Extract Service HTTP API"
                });
            });
            return services;
        }

        public static IServiceCollection AddPublicationService(this IServiceCollection services, IConfiguration configuration)
        {
            var storageConnectionString = configuration["StorageConnectionString"];
            var tableName = configuration["PublicationTableName"];

            services.AddScoped<IPublicationsService>(opt =>
            {
                return new PublicationsService(new PublicationRepository(storageConnectionString, tableName), ConfigureAutoMapper());
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
