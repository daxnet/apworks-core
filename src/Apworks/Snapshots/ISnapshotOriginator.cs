using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Snapshots
{
    public interface ISnapshotOriginator
    {
        ISnapshot TakeSnapshot();

        void RestoreSnapshot(ISnapshot snapshot);
    }
}
