using Apworks.Messaging;
using Apworks.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Messaging
{
    public sealed class ServiceProviderMessageHandlerExecutionContext : MemoryBasedMessageHandlerExecutionContext
    {
        private readonly IServiceCollection registry;
        private readonly Func<IServiceCollection, IServiceProvider> serviceProviderFactory;

        public ServiceProviderMessageHandlerExecutionContext(IServiceCollection registry,
            Func<IServiceCollection, IServiceProvider> serviceProviderFactory = null)
        {
            this.registry = registry;
            if (serviceProviderFactory == null)
            {
                this.serviceProviderFactory = sc => registry.BuildServiceProvider();
            }
            else
            {
                this.serviceProviderFactory = serviceProviderFactory;
            }
        }

        public override void RegisterHandler(Type messageType, Type handlerType)
        {
            base.RegisterHandler(messageType, handlerType);

            this.registry.AddTransient(handlerType);
        }

        public override async Task HandleMessageAsync(IMessage message, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var messageType = message.GetType();

            // Checks if the handlers of the current message have already been registered.
            if (registrations.TryGetValue(messageType, out List<Type> handlerTypes) &&
                handlerTypes?.Count > 0)
            {
                var serviceProvider = this.serviceProviderFactory(this.registry);

                // Creates the child scope for the message handlers so that the dependencies can be disposed
                // after the message has been handled.
                using (var childScope = serviceProvider.CreateScope())
                {
                    foreach(var handlerType in handlerTypes)
                    {
                        var handler = (IMessageHandler)childScope.ServiceProvider.GetService(handlerType);
                        if (handler.CanHandle(messageType))
                        {
                            await handler.HandleAsync(message, cancellationToken);
                        }
                    }
                }
            }
        }
    }
}
