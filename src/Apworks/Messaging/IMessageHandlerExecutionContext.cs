using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    public interface IMessageHandlerExecutionContext
    {
        void RegisterHandler<TMessage, THandler>()
            where TMessage : IMessage
            where THandler : IMessageHandler<TMessage>;

        void RegisterHandler(Type messageType, Type handlerType);

        Task HandleMessageAsync(IMessage message, CancellationToken cancellationToken = default(CancellationToken));
    }
}
