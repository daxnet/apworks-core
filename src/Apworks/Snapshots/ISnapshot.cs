using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Snapshots
{
    /// <summary>
    /// Represents that the implemented classes are aggregate snapshots.
    /// </summary>
    public interface ISnapshot
    {
        /// <summary>
        /// Gets or sets the date and time on which the snapshot has been taken.
        /// </summary>
        DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the version of the aggregate root on which the snapshot has been taken.
        /// </summary>
        long Version { get; set; }

        /// <summary>
        /// Adds the state.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="state">The state.</param>
        void AddState(string key, object state);

        /// <summary>
        /// Gets the states of the aggregate when the snapshot has been taken.
        /// </summary>
        IEnumerable<KeyValuePair<string, object>> States { get; }
    }
}
