using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Dynamic;
using System.Text;

namespace Apworks.Serialization.Json
{
    public sealed class ObjectJsonSerializer : ObjectSerializer
    {
        private readonly Encoding encoding;
        private readonly JsonSerializerSettings settings;

        public ObjectJsonSerializer(JsonSerializerSettings settings = null, Encoding encoding = null)
        {
            this.settings = settings;
            if (this.settings != null)
            {
                this.settings.Converters.Add(new ExpandoObjectConverter());
            }
            else
            {
                this.settings = new JsonSerializerSettings { Converters = new[] { new ExpandoObjectConverter() } };
            }
            
            this.encoding = encoding ?? Encoding.UTF8;
        }

        public override dynamic Deserialize(Type objType, byte[] data)
        {
            var json = this.encoding.GetString(data);
            return JsonConvert.DeserializeObject(json, objType, settings);
        }

        public override dynamic Deserialize(byte[] data)
        {
            var json = this.encoding.GetString(data);
            return JsonConvert.DeserializeObject<ExpandoObject>(json, this.settings);
        }

        public override byte[] Serialize(Type objType, object obj)
        {
            var json = JsonConvert.SerializeObject(obj, objType, this.settings);
            return this.encoding.GetBytes(json);
        }

        public override byte[] Serialize(object @object)
        {
            var json = JsonConvert.SerializeObject(@object, Formatting.Indented, this.settings);
            return this.encoding.GetBytes(json);
        }
    }
}
