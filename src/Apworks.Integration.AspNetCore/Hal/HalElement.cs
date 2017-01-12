using Apworks.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Hal
{
    public abstract class HalElement : IHalElement
    {
        private int indentation;

        public string ToJson(HalGenerationOption option)
        {
            this.indentation = 0;
            var result = new StringBuilder();
            using (var stringWriter = new StringWriter(result))
            {
                this.WriteJson(stringWriter, option);
            }

            return result.ToString();
        }

        public virtual string ToXml(HalGenerationOption option)
        {
            return @"<?xml version=""1.0"" encoding=""UTF-8""?>";
        }

        public override string ToString() => ToJson(HalGenerationOption.Default);

        protected abstract void WriteJson(StringWriter writer, HalGenerationOption option);

        protected void WriteStartObject(StringWriter writer, HalGenerationOption option)
        {
            WriteStartTag("{", writer, option);
        }

        protected void WriteEndObject(StringWriter writer, HalGenerationOption option)
        {
            WriteEndTag("}", writer, option);
        }

        protected void WriteStartArray(StringWriter writer, HalGenerationOption option)
        {
            WriteStartTag("[", writer, option);
        }

        protected void WriteEndArray(StringWriter writer, HalGenerationOption option)
        {
            WriteEndTag("]", writer, option);
        }

        protected void WritePropertyValue(string propertyName, object propertyValue, StringWriter writer, HalGenerationOption option, bool withEndingComma = true)
        {
            var propertyValueStr = string.Empty;
            if (propertyValue == null)
            {
                if (option.IgnoreNullValues)
                {
                    return;
                }
                else
                {
                    propertyValueStr = "null";
                }
            }
            else
            {
                var objectType = propertyValue.GetType();
                if (objectType.IsSimpleType())
                {
                    if (objectType == typeof(string) || objectType == typeof(DateTime) || objectType == typeof(DateTime?))
                    {
                        propertyValueStr = $"\"{propertyValue.ToString()}\"";
                    }
                    else
                    {
                        propertyValueStr = propertyValue.ToString().ToLower();
                    }
                }
                else
                {
                    return;
                }
            }

            var ending = withEndingComma ? ", " : "";
            switch(option.Format)
            {
                case HalFormat.Formatted:
                    WriteIdentation(writer);
                    writer.WriteLine($"\"{propertyName}\": {propertyValueStr}{ending}");
                    break;
                default:
                    writer.Write($"\"{propertyName}\": {propertyValueStr}{ending}");
                    break;
            }
        }

        private void WriteIdentation(StringWriter writer)
        {
            for(var i=0;i<this.indentation;i++)
            {
                writer.Write("\t");
            }
        }

        private void WriteStartTag(string tag, StringWriter writer, HalGenerationOption option)
        {
            switch (option.Format)
            {
                case HalFormat.Formatted:
                    WriteIdentation(writer);
                    writer.WriteLine(tag);
                    this.indentation++;
                    break;
                default:
                    writer.Write(tag);
                    break;
            }
        }

        private void WriteEndTag(string tag, StringWriter writer, HalGenerationOption option)
        {
            switch (option.Format)
            {
                case HalFormat.Formatted:
                    this.indentation--;
                    WriteIdentation(writer);
                    writer.WriteLine(tag);
                    break;
                default:
                    writer.Write(tag);
                    break;
            }
        }
    }
}
