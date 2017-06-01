using System;
using System.Collections.Generic;
using System.Text;

namespace Apworks.Integration.AspNetCore.Hal
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class HalEmbeddedResourceAttribute : Attribute
    {
        public HalEmbeddedResourceAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}
