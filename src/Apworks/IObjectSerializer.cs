using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks
{
    public interface IObjectSerializer
    {
        byte[] Serialize<TObject>(TObject obj);

        Task<byte[]> SerializeAsync<TObject>(TObject obj, CancellationToken cancellationToken = default(CancellationToken));

        byte[] Serialize(Type objType, object obj);

        Task<byte[]> SerializeAsync(Type objType, object obj, CancellationToken cancellationToken = default(CancellationToken));

        TObject Deserialize<TObject>(byte[] data);

        Task<TObject> DeserializeAsync<TObject>(byte[] data, CancellationToken cancellationToken = default(CancellationToken));

        object Deserialize(Type objType, byte[] data);

        Task<object> DeserializeAsync(Type objType, byte[] data, CancellationToken cancellationToken = default(CancellationToken));
    }
}
