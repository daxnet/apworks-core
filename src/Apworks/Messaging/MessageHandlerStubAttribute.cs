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
        {
            this.TargetType = targetType;
        }

        public Type TargetType { get; }
    }
}
