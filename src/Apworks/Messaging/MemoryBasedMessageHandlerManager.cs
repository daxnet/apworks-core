using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    public abstract class MemoryBasedMessageHandlerManager : MessageHandlerManager
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

        public override bool HasHandlersRegistered(Type messageType)
        {
            if (registrations.TryGetValue(messageType, out List<Type> registeredHandlerTypes))
            {
                return registeredHandlerTypes?.Count > 0;
            }
            else
            {
                return false;
            }
        }

        public override bool HasRegistered(Type messageType, Type handlerType)
        {
            if (registrations.TryGetValue(messageType, out List<Type> registeredHandlerTypes))
            {
                var validation = registeredHandlerTypes?.Contains(handlerType);
                return validation.HasValue && validation.Value;
            }
            else
            {
                return false;
            }
        }
    }
}
