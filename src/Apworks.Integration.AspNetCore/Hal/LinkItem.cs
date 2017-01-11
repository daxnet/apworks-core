using Apworks.Integration.AspNetCore.Hal.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Hal
{
    public sealed class LinkItem : ILinkItem
    {
        private readonly Dictionary<string, object> properties = new Dictionary<string, object>();

        [JsonProperty(PropertyName = "href", NullValueHandling = NullValueHandling.Ignore)]
        public string Href { get; set; }

        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "templated", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Templated { get; set; }

        public IEnumerable<KeyValuePair<string, object>> Properties => properties;

        public void AddProperty(string name, object value)
        {
            this.properties.Add(name, value);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new FlattenConverter());
        }

        public string ToXml()
        {
            throw new NotImplementedException();
        }
    }
}
