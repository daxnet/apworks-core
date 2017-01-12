using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Apworks.Integration.AspNetCore.Hal
{
    public sealed class LinkItemCollection : HalElement, ICollection<ILinkItem>
    {
        private readonly List<ILinkItem> items = new List<ILinkItem>();

        public int Count => items.Count;

        public bool IsReadOnly => false;

        public void Add(ILinkItem item) => items.Add(item);

        public void Clear() => items.Clear();

        public bool Contains(ILinkItem item) => items.Contains(item);

        public void CopyTo(ILinkItem[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

        public IEnumerator<ILinkItem> GetEnumerator() => items.GetEnumerator();

        public bool Remove(ILinkItem item) => items.Remove(item);

        protected override void WriteJson(StringWriter writer, HalGenerationOption option)
        {
            if (this.items.Count == 1)
            {
                switch(option.Format)
                {
                    case HalFormat.Formatted:
                        writer.WriteLine(this.items[0].ToJson(option));
                        break;
                    default:
                        writer.Write(this.items[0].ToJson(option));
                        break;
                }
            }
            else
            {
                WriteStartArray(writer, option);
                foreach(var item in items)
                {
                    switch (option.Format)
                    {
                        case HalFormat.Formatted:
                            writer.WriteLine(item.ToJson(option));
                            break;
                        default:
                            writer.Write(item.ToJson(option));
                            break;
                    }
                }
                WriteEndArray(writer, option);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
    }
}
