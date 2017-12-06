using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class MessageHandlerStubAttribute : Attribute
    {
        public MessageHandlerStubAttribute(Type targetType)
            : this(targetType, 0)
        {

        }

        public MessageHandlerStubAttribute(Type targetType, int priority)
        {
            this.TargetType = targetType;
            this.Priority = priority;
        }

        public Type TargetType { get; }

        public int Priority { get; }
    }
}
