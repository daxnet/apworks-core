using Apworks.Utilities;
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
            Utils.ConcurrentDictionarySafeRegister(messageType, handlerType, this.registrations);
        }

        public override bool HandlerRegistered(Type messageType, Type handlerType)
        {
            if (this.registrations.TryGetValue(messageType, out List<Type> handlerTypeList))
            {
                return handlerTypeList != null && handlerTypeList.Contains(handlerType);
            }

            return false;
        }
    }
}