using Apworks.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Apworks.Integration.AspNetCore.Messaging
{
    public sealed class MessageHandlerProvider : MemoryBasedMessageHandlerManager
    {
        private readonly IServiceCollection registry;

        public MessageHandlerProvider(IServiceCollection registry)
        {
            this.registry = registry;
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

            this.registry.AddTransient(handlerStubType, handlerType);
        }

        protected override IEnumerable<IMessageHandler> ResolveHandler(Type messageType)
        {
            if (registrations.TryGetValue(messageType, out List<Type> handlerTypes) &&
                handlerTypes?.Count > 0)
            {
                var serviceProvider = this.registry.BuildServiceProvider();
                var ret = new List<IMessageHandler>();
                var cachedHandlerStubType = new List<Type>();
                foreach(var handlerType in handlerTypes.Distinct())
                {
                    var (handlerStubType, __) = EvaluateMessageHandlerStub(handlerType);
                    if (!cachedHandlerStubType.Contains(handlerStubType))
                    {
                        serviceProvider.GetServices(handlerStubType)
                            .ToList()
                            .ForEach(service => 
                                ret.Add(service as IMessageHandler));
                        cachedHandlerStubType.Add(handlerStubType);
                    }
                }

                return ret;
            }

            return null;
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
