using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.KeyGeneration
{
    public sealed class GuidKeyGenerator : IKeyGenerator<Guid, IAggregateRoot<Guid>>
    {
        public Guid Generate(IAggregateRoot<Guid> aggregateRoot)
        {
            return Guid.NewGuid();
        }
    }
}
