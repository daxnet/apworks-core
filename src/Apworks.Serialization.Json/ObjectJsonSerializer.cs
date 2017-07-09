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
            if (this.settings != null)
            {
                var typeHandling = this.settings.TypeNameHandling;
                try
                {
                    this.settings.TypeNameHandling = TypeNameHandling.All;
                    var json = this.encoding.GetString(data);
                    return JsonConvert.DeserializeObject(json, this.settings);
                }
                finally
                {
                    this.settings.TypeNameHandling = typeHandling;
                }
            }
            else
            {
                var json = this.encoding.GetString(data);
                return JsonConvert.DeserializeObject(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            }
        }

        public override byte[] Serialize(Type objType, object obj)
        {
            var json = JsonConvert.SerializeObject(obj, objType, this.settings);
            return this.encoding.GetBytes(json);
        }

        public override byte[] Serialize(object @object)
        {
            if (this.settings != null)
            {
                var typeHandling = this.settings.TypeNameHandling;
                try
                {
                    this.settings.TypeNameHandling = TypeNameHandling.All;
                    var json = JsonConvert.SerializeObject(@object, Formatting.Indented, this.settings);
                    return this.encoding.GetBytes(json);
                }
                finally
                {
                    this.settings.TypeNameHandling = typeHandling;
                }
            }
            else
            {
                var json = JsonConvert.SerializeObject(@object, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                return this.encoding.GetBytes(json);
            }
        }
    }
}
