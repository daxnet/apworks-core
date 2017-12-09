using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    public abstract class MemoryBasedMessageHandlerExecutionContext : MessageHandlerExecutionContext
    {
        protected readonly ConcurrentDictionary<Type, List<Type>> registrations = new ConcurrentDictionary<Type, List<Type>>();

        public override void RegisterHandler(Type messageType, Type handlerType)
        {
            if (registrations.TryGetValue(messageType, out List<Type> registeredHandlerTypes))
            {
                if (registeredHandlerTypes != null)
                {
                    registrations[messageType].Add(handlerType);
                }
                else
                {
                    registrations[messageType] = new List<Type> { handlerType };
                }
            }
            else
            {
                registrations.TryAdd(messageType, new List<Type> { handlerType });
            }
        }
    }
}
