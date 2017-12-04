using Apworks.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apworks.Integration.AspNetCore.Messaging
{
    public sealed class MessageHandlerProvider : MemoryBasedMessageHandlerManager
    {
        private readonly IServiceCollection registry;
        private readonly IServiceProvider provider;

        public MessageHandlerProvider(IServiceCollection registry)
        {
            this.registry = registry;
            this.provider = registry.BuildServiceProvider();
        }

        public override void RegisterHandler(Type messageType, Type handlerType)
        {
            base.RegisterHandler(messageType, handlerType);

            throw new NotImplementedException();
        }

        protected override IEnumerable<IMessageHandler> ResolveHandler(Type messageType)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
