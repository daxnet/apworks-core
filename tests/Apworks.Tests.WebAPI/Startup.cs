using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Apworks.Commands;
using Apworks.Messaging.RabbitMQ;
using RabbitMQ.Client;
using Apworks.Serialization.Json;
using System.IO;
using Apworks.Messaging;
using Apworks.Integration.AspNetCore.Messaging;
using Apworks.Tests.WebAPI.Commands;
using Apworks.Tests.WebAPI.CommandHandlers;
using Apworks.Events;
using Apworks.EventStore.SQLServer;
using Apworks.EventStore.AdoNet;
using Apworks.Snapshots;
using Apworks.Repositories;

namespace Apworks.Tests.WebAPI
{
    public class Startup
    {
        private const string RabbitExchangeName = "Apworks.Tests.WebAPI";
        private const string EventStoreConnectionString = @"Server=localhost\sqlexpress; Database=ApworksTestWebAPIDatabase; Integrated Security=SSPI;";
        private readonly ILogger logger;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            var LogFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Apworks.Tests.WebAPI.log.txt");
            Configuration = configuration;

            this.logger = loggerFactory.CreateLogger<Startup>();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            logger.LogInformation("Configuring application services.");

            services.AddMvc();

            var rabbitMQConnectionFactory = new ConnectionFactory() { HostName = "localhost" };
            var messageSerializer = new MessageJsonSerializer();

            var commandHandlerExecutionContext = new ServiceProviderMessageHandlerExecutionContext(services, sc => sc.BuildServiceProvider());
            commandHandlerExecutionContext.RegisterHandler<CreateCustomerCommand, CreateCustomerCommandHandler>();

            var commandBus = new RabbitCommandBus(rabbitMQConnectionFactory, messageSerializer, RabbitExchangeName);

            services.AddSingleton<IMessageHandlerExecutionContext>(commandHandlerExecutionContext);
            services.AddSingleton<ICommandSender>(commandBus);
            services.AddSingleton<ICommandSubscriber>(commandBus);
            services.AddSingleton<ICommandConsumer>(serviceProvider => new CommandConsumer(serviceProvider.GetRequiredService<ICommandSubscriber>(), commandHandlerExecutionContext));

            var adonetConfig = new AdoNetEventStoreConfiguration(EventStoreConnectionString);
            var objectSerializer = new ObjectJsonSerializer();

            services.AddTransient<IEventStore>(serviceProvider => new SqlServerEventStore(adonetConfig, objectSerializer));
            services.AddTransient<IEventPublisher>(serviceProvider => new RabbitEventBus(rabbitMQConnectionFactory, messageSerializer, RabbitExchangeName));
            services.AddSingleton<ISnapshotProvider, SuppressedSnapshotProvider>();

            services.AddTransient<IDomainRepository, EventSourcingDomainRepositoryWithLogging>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            var applicationServices = app.ApplicationServices;

            // Hook the registered services to dump the information about the disposing.
            var commandSender = applicationServices.GetRequiredService<ICommandSender>();
            ((DisposableObject)commandSender).Disposed += (s1, e1) =>
            {
                logger.LogInformation("Command Sender has disposed.");
            };

            var commandSubscriber = applicationServices.GetRequiredService<ICommandSubscriber>();
            ((DisposableObject)commandSubscriber).Disposed += (s2, e2) =>
            {
                logger.LogInformation("Command Subscriber has disposed.");
            };

            var commandConsumer = applicationServices.GetRequiredService<ICommandConsumer>();
            ((DisposableObject)commandConsumer).Disposed += (s3, e3) =>
            {
                logger.LogInformation("Command Consumer has disposed.");
            };

            commandConsumer.Consume();

            lifetime.ApplicationStopping.Register(() =>
            {
                commandSender.Dispose();
                commandSubscriber.Dispose();
                commandConsumer.Dispose();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
