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

        [JsonProperty(PropertyName = "href")]
        public string Href { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "templated")]
        public bool? Templated { get; set; }

        public IEnumerable<KeyValuePair<string, object>> Properties => properties;

        public void AddProperty(string name, object value)
        {
            this.properties.Add(name, value);
        }

        public string ToJson(HalGenerationOption option)
        {
            var settings = option.ToSerializerSettings();
            settings.Converters.Add(new ObjectFlattenConverter());
            return JsonConvert.SerializeObject(this, settings);
        }

        public string ToXml(HalGenerationOption option)
        {
            throw new NotImplementedException();
        }
    }
}
