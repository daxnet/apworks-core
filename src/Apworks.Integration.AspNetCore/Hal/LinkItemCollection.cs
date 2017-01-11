using Apworks.Integration.AspNetCore.Hal.Converters;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Hal
{
    [JsonArray(ItemConverterType = typeof(ObjectFlattenConverter))]
    public sealed class LinkItemCollection : ICollection<ILinkItem>, IHalElement
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

        public string ToJson(HalGenerationOption option)
        {
            var settings = option.ToSerializerSettings();
            settings.Converters.Add(new ArrayReductionConverter());
            return JsonConvert.SerializeObject(this, settings);
        }

        public string ToXml(HalGenerationOption option)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
    }
}
