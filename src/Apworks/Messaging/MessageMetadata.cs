using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    /// <summary>
    /// Represents the object that contains the metadata information for a particular message.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IDictionary{System.String, System.Object}" />
    public sealed class MessageMetadata : IDictionary<string, object>
    {
        // The data bank which holds the message metadata.
        private readonly Dictionary<string, object> bank = new Dictionary<string, object>();

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public object this[string key]
        {
            get => this.bank.ContainsKey(key) ? this.bank[key] : null;
            set => this.bank[key] = value;
        }

        public ICollection<string> Keys => bank.Keys;

        public ICollection<object> Values => bank.Values;

        public int Count => bank.Count;

        public bool IsReadOnly => false;

        /// <summary>
        /// Safely adds a metadata value with the specified key. If the key already exists, the adding
        /// of the value will be ignored.
        /// </summary>
        /// <param name="key">The key of the metadata value to be added.</param>
        /// <param name="value">The value of the metadata.</param>
        public void Add(string key, object value)
        {
            if (!this.bank.ContainsKey(key))
            {
                this.bank.Add(key, value);
            }
        }

        /// <summary>
        /// Safely adds a metadata value with the specified key. If the key already exists, the adding
        /// of the value will be ignored.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void Add(KeyValuePair<string, object> item) => Add(item.Key, item.Value);

        public void Clear() => bank.Clear();

        public bool Contains(KeyValuePair<string, object> item) => bank.Contains(item);

        public bool ContainsKey(string key) => bank.ContainsKey(key);

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => bank.ToArray().CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => bank.GetEnumerator();

        public bool Remove(string key) => bank.Remove(key);

        public bool Remove(KeyValuePair<string, object> item) => bank.Remove(item.Key);

        public bool TryGetValue(string key, out object value) => bank.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => bank.GetEnumerator();
    }
}
