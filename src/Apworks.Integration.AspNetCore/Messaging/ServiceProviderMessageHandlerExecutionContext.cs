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
        private readonly ConcurrentDictionary<Type, List<Type>> messageHandlerStubTypeMapping =
            new ConcurrentDictionary<Type, List<Type>>();

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

            var (handlerStubType, handlerTargetType) = EvaluateMessageHandlerStub(handlerType);

            if (handlerStubType == null ||
                handlerTargetType == null)
            {
                throw new MessageProcessingException($"Cannot evaluate the base interface type for the message handler {handlerType.FullName}.");
            }

            if (!messageType.GetInterfaces().Contains(handlerTargetType))
            {
                throw new MessageProcessingException($"The message handler type {handlerType.FullName} is not expected to handler the message with the type ${messageType.FullName}.");
            }

            // Registers the relationship between the message type and its handler stub type.
            Utils.ConcurrentDictionarySafeRegister(messageType, handlerStubType, this.messageHandlerStubTypeMapping);

            this.registry.AddTransient(handlerStubType, handlerType);
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
                    if (messageHandlerStubTypeMapping.TryGetValue(messageType, out List<Type> handlerStubTypes))
                    {
                        foreach(var handlerStubType in handlerStubTypes)
                        {
                            var messageHandlers = childScope.ServiceProvider.GetServices(handlerStubType);
                            foreach(var obj in messageHandlers)
                            {
                                if (obj is IMessageHandler messageHandler && messageHandler.CanHandle(messageType))
                                {
                                    await messageHandler.HandleAsync(message, cancellationToken);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Evaluates the message handler stub.
        /// </summary>
        /// <param name="handlerType">Type of the handler.</param>
        /// <returns>A tuple which contains the type of the message handler stub (an interface) and
        /// the type of the message base type (an interface) which the message handler stub would
        /// be able to handle.</returns>
        /// <exception cref="NotImplementedException"></exception>
        private static (Type handlerStubType, Type handlerTargetType) EvaluateMessageHandlerStub(Type handlerType)
        {
            var query = from it in handlerType.GetTypeInfo().GetInterfaces()
                        let typeInfo = it.GetTypeInfo()
                        where typeInfo.IsDefined(typeof(MessageHandlerStubAttribute))
                        let messageHandlerStubAttribute = typeInfo.GetCustomAttribute<MessageHandlerStubAttribute>()
                        orderby messageHandlerStubAttribute.Priority descending
                        select new
                        {
                            HandlerStubType = it,
                            HandlerTargetType = messageHandlerStubAttribute.TargetType
                        };

            var handlerStubInfo = query.FirstOrDefault();
            return (handlerStubInfo?.HandlerStubType, handlerStubInfo?.HandlerTargetType);
        }
    }
}
