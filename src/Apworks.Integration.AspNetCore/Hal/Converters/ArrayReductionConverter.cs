using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Hal.Converters
{
    internal sealed class ArrayReductionConverter : JsonConverter
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
            JToken token = JToken.FromObject(value);
            if (token.Type != JTokenType.Array)
            {
                token.WriteTo(writer);
            }
            else
            {
                var arr = (JArray)token;
                if (arr.Count > 1)
                {
                    arr.WriteTo(writer);
                }
                else
                {
                    arr[0].WriteTo(writer);
                }
            }
        }

        public override bool CanRead => false;
    }
}
