using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Repositories.DictionaryRepository
{
    public sealed class DictionaryRepositoryContext : RepositoryContext<Dictionary<object, object>>
    {
        public DictionaryRepositoryContext(Dictionary<object, object> session) : base(session)
        {
        }

        protected override IRepository<TKey, TAggregateRoot> CreateRepository<TKey, TAggregateRoot>()
        {
            throw new NotImplementedException();
        }
    }
}
