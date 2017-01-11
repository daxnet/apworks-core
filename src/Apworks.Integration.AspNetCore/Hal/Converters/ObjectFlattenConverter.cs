using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Hal.Converters
{
    internal sealed class ObjectFlattenConverter : JsonConverter
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
                WriteJson(writer, obj, serializer);
                writer.WriteEndObject();
            }
        }

        public override bool CanRead => false;

        private void WriteJson(JsonWriter writer, JObject value, JsonSerializer serializer)
        {
            foreach (var property in value.Properties())
            {
                var jObject = property.Value as JObject;
                if (jObject != null)
                    WriteJson(writer, jObject, serializer);
                else
                {
                    var v = (JValue)property.Value;
                    if (v.Value == null && serializer.NullValueHandling == NullValueHandling.Ignore)
                    {
                        continue;
                    }

                    property.WriteTo(writer);
                }
            }
        }
    }
}
