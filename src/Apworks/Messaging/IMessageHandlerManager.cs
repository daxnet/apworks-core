using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    public interface IMessageHandlerManager
    {
        void RegisterHandler<TMessage, THandler>()
            where TMessage : IMessage
            where THandler : IMessageHandler<TMessage>;

        void RegisterHandler(Type messageType, Type handlerType);

        bool HasHandlersRegistered<TMessage>()
            where TMessage : IMessage;

        bool HasHandlersRegistered(Type messageType);

        bool HasRegistered<TMessage, THandler>()
            where TMessage : IMessage
            where THandler : IMessageHandler<TMessage>;

        bool HasRegistered(Type messageType, Type handlerType);

        IEnumerable<IMessageHandler> GetHandlersFor<TMessage>()
            where TMessage : IMessage;

        IEnumerable<IMessageHandler> GetHandlersFor(Type messageType);
    }
}
