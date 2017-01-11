using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Hal.Converters
{
    internal sealed class FlattenConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var token = JToken.FromObject(value);

            if (token.Type != JTokenType.Object)
            {
                token.WriteTo(writer);
            }
            else
            {
                var obj = (JObject)token;
                writer.WriteStartObject();
                WriteJson(writer, obj);
                writer.WriteEndObject();
            }
        }

        public override bool CanRead => false;

        private void WriteJson(JsonWriter writer, JObject value)
        {
            foreach (var property in value.Properties())
            {
                var jObject = property.Value as JObject;
                if (jObject != null)
                    WriteJson(writer, jObject);
                else
                    property.WriteTo(writer);
            }
        }
    }
}
