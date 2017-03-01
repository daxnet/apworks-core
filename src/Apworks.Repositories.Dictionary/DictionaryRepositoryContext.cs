using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Repositories.Dictionary
{
    public sealed class DictionaryRepositoryContext : RepositoryContext<ConcurrentDictionary<object, object>>
    {
        public DictionaryRepositoryContext(ConcurrentDictionary<object, object> session) : base(session)
        {
        }

        protected override IRepository<TKey, TAggregateRoot> CreateRepository<TKey, TAggregateRoot>() =>
            new DictionaryRepository<TKey, TAggregateRoot>(this);
    }
}
