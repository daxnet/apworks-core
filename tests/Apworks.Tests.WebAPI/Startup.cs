using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Apworks.Commands;
using Apworks.Messaging.RabbitMQ;
using RabbitMQ.Client;
using Apworks.Serialization.Json;
using System.IO;
using Apworks.Messaging;
using Apworks.Integration.AspNetCore.Messaging;
using Apworks.Tests.WebAPI.Commands;
using Apworks.Tests.WebAPI.CommandHandlers;

namespace Apworks.Tests.WebAPI
{
    public class Startup
    {
        private const string RabbitExchangeName = "Apworks.Tests.WebAPI";
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

            var commandHandlerManager = new MessageHandlerProvider(services, sc => sc.BuildServiceProvider());
            commandHandlerManager.RegisterHandler<CreateCustomerCommand, CreateCustomerCommandHandler>();

            var commandBus = new CommandBus(rabbitMQConnectionFactory, messageSerializer, RabbitExchangeName);

            services.AddSingleton<ICommandSender>(commandBus);
            services.AddSingleton<ICommandSubscriber>(commandBus);
            services.AddSingleton<ICommandConsumer>(serviceProvider => new CommandConsumer(serviceProvider.GetRequiredService<ICommandSubscriber>(), commandHandlerManager));

            // Hook the registered services to dump the information about the disposing.
            var sp = services.BuildServiceProvider();
            var commandSender = sp.GetService<ICommandSender>();
            ((DisposableObject)commandSender).Disposed += (s1, e1) =>
              {
                  logger.LogInformation("Command Sender has disposed.");
              };

            var commandSubscriber = sp.GetService<ICommandSubscriber>();
            ((DisposableObject)commandSubscriber).Disposed += (s2, e2) =>
             {
                 logger.LogInformation("Command Subscriber has disposed.");
             };

            var commandConsumer = sp.GetService<ICommandConsumer>();
            ((DisposableObject)commandConsumer).Disposed += (s2, e2) =>
            {
                logger.LogInformation("Command Consumer has disposed.");
            };
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            var applicationServices = app.ApplicationServices;
            var commandConsumer = applicationServices.GetRequiredService<ICommandConsumer>();

            commandConsumer.Consume();

            lifetime.ApplicationStopping.Register(() =>
            {
                var commandSender = app.ApplicationServices.GetRequiredService<ICommandSender>();
                commandSender.Dispose();

                var commandSubscriber = app.ApplicationServices.GetRequiredService<ICommandSubscriber>();
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
