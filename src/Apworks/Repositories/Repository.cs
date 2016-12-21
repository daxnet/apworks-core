using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Repositories
{
    public abstract class Repository<TKey, TAggregateRoot> : IRepository<TKey, TAggregateRoot>
        where TKey : IEquatable<TKey>
        where TAggregateRoot : IAggregateRoot<TKey>
    {
        protected Repository(IRepositoryContext context)
        {
            this.Context = context;
        }

        public IRepositoryContext Context { get; }

        public abstract void Add(TAggregateRoot aggregateRoot);

        public abstract Task AddAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken = default(CancellationToken));

        public abstract TAggregateRoot FindByKey(TKey key);

        public abstract Task<TAggregateRoot> FindByKeyAsync(TKey key, CancellationToken cancellationToken = default(CancellationToken));
    }
}
