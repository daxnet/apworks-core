using System;
using System.Collections.Generic;
using System.IO;

namespace Apworks.Integration.AspNetCore.Hal
{
    public sealed class LinkItem : HalElement, ILinkItem
    {
        private readonly Dictionary<string, object> properties = new Dictionary<string, object>();

        public string Href { get; set; }

        public string Name { get; set; }

        public bool? Templated { get; set; }

        public IEnumerable<KeyValuePair<string, object>> Properties => properties;

        public void AddProperty(string name, object value)
        {
            this.properties.Add(name, value);
        }

        protected override void WriteJson(StringWriter writer, HalGenerationOption option)
        {
            WriteStartObject(writer, option);
            WritePropertyValue("href", this.Href, writer, option);
            WritePropertyValue("name", this.Name, writer, option);
            WritePropertyValue("templated", this.Templated, writer, option);

            var idx = 0;
            foreach(var property in this.properties)
            {
                WritePropertyValue(property.Key, property.Value, writer, option, idx++ != this.properties.Count - 1);
            }

            WriteEndObject(writer, option);
        }
    }
}
