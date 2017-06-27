using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Snapshots
{
    public sealed class Snapshot : ISnapshot
    {
        private readonly Dictionary<string, object> states = new Dictionary<string, object>();

        public Snapshot() { }

        public DateTime Timestamp { get; set; }

        public long Version { get; set; }

        public IEnumerable<KeyValuePair<string, object>> States => this.states;

        public void AddState(string key, object state)
        {
            this.states[key] = state;
        }
    }
}
