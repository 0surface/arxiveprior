using arx.Extract.BackgroundTasks.Core;
using arx.Extract.Data.Repository;
using arx.Extract.Lib;
using Autofac;
using AutoMapper;
using EventBus;
using EventBus.Abstractions;
using EventBusRabbitMQ;
using EventBusServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;

namespace arx.Extract.BackgroundTasks.Extensions
{
    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Type assemblyMarkerType)
        {
            services.AddAutoMapper(c => c.AddProfile<BackgroundTasksAutoMapperProfile>(), assemblyMarkerType);
            return services;
        }
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var subscriptionClientName = configuration["SubscriptionClientName"];

            if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
            {
                services.AddSingleton<IServiceBusPersisterConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultServiceBusPersisterConnection>>();

                    var serviceBusConnectionString = configuration["EventBusConnection"];
                    var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);

                    return new DefaultServiceBusPersisterConnection(serviceBusConnection, logger);
                });

                services.AddSingleton<IEventBus, AzureServiceBus>(sp =>
                {
                    var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var logger = sp.GetRequiredService<ILogger<AzureServiceBus>>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                    return new AzureServiceBus(serviceBusPersisterConnection, logger, eventBusSubcriptionsManager, subscriptionClientName, iLifetimeScope);
                });
            }
            else
            {
                services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                    var factory = new ConnectionFactory()
                    {
                        HostName = configuration["EventBusConnection"],
                        DispatchConsumersAsync = true
                    };

                    if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
                    {
                        factory.UserName = configuration["EventBusUserName"];
                    }

                    if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
                    {
                        factory.Password = configuration["EventBusPassword"];
                    }
                    factory.Port = AmqpTcpEndpoint.UseDefaultPort;

                    var retryCount = 5;

                    if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                    {
                        retryCount = int.Parse(configuration["EventBusRetryCount"]);
                    }

                    return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
                });

                services.AddSingleton<IEventBus, EventBusRabbitMQ.EventBusRabbitMQ>(sp =>
               {
                   var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                   var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                   var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ.EventBusRabbitMQ>>();
                   var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                   var retryCount = 5;

                   if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                   {
                       retryCount = int.Parse(configuration["EventBusRetryCount"]);
                   }

                   return new EventBusRabbitMQ.EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
               });
            }

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            return services;
        }

        public static IServiceCollection AddSubjectRepository(this IServiceCollection services, IConfiguration configuration)
        {
            var storageConnectionString = configuration["StorageConnectionString"];
            var subjectTableName = configuration["SubjectTableName"];
            services.AddSingleton<ISubjectRepository>(opt =>
            {
                return new SubjectRepository(storageConnectionString, subjectTableName);
            });
            return services;
        }

        public static IServiceCollection AddPublicationRepository(this IServiceCollection services, IConfiguration configuration)
        {
            var storageConnectionString = configuration["StorageConnectionString"];
            var tableName = configuration["PublicationTableName"];
            services.AddSingleton<IPublicationRepository>(opt =>
            {
                return new PublicationRepository(storageConnectionString, tableName);
            });
            return services;
        }

        public static IServiceCollection AddJobRepository(this IServiceCollection services, IConfiguration configuration)
        {
            var storageConnectionString = configuration["StorageConnectionString"];
            var tableName = configuration["JobTableName"];
            services.AddSingleton<IJobRepository>(opt =>
            {
                return new JobRepository(storageConnectionString, tableName);
            });
            return services;
        }

        public static IServiceCollection AddJobItemRepository(this IServiceCollection services, IConfiguration configuration)
        {
            var storageConnectionString = configuration["StorageConnectionString"];
            var tableName = configuration["JobItemTableName"];
            services.AddSingleton<IJobItemRepository>(opt =>
            {
                return new JobItemRepository(storageConnectionString, tableName);
            });
            return services;
        }

        public static IServiceCollection AddFulfilmentRepository(this IServiceCollection services, IConfiguration configuration)
        {
            var storageConnectionString = configuration["StorageConnectionString"];
            var tableName = configuration["FulfilmentTableName"];
            services.AddSingleton<IFulfilmentRepository>(opt =>
            {
                return new FulfilmentRepository(storageConnectionString, tableName);
            });
            return services;
        }

        public static IServiceCollection AddFulfilmentItemRepository(this IServiceCollection services, IConfiguration configuration)
        {
            var storageConnectionString = configuration["StorageConnectionString"];
            var tableName = configuration["FulfilmentItemTableName"];
            services.AddSingleton<IFulfilmentItemRepository>(opt =>
            {
                return new FulfilmentItemRepository(storageConnectionString, tableName);
            });
            return services;
        }

        public static IServiceCollection AddArchiveFetch(this IServiceCollection services)
        {
            services.AddSingleton<IArchiveFetch, ArchiveFetch>();
            return services;
        }

        public static IServiceCollection AddTransformService(this IServiceCollection services)
        {
            services.AddSingleton<ITransformService, TransformService>();
            return services;
        }
    }
}
