using Newtonsoft.Json;
using System;
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
            this.encoding = encoding ?? Encoding.UTF8;
        }

        public override object Deserialize(Type objType, byte[] data)
        {
            var json = this.encoding.GetString(data);
            return JsonConvert.DeserializeObject(json, objType, settings);
        }

        public override object Deserialize(byte[] data)
        {
            var json = this.encoding.GetString(data);
            return JsonConvert.DeserializeObject(json, this.settings);
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
