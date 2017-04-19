using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Repositories
{
    public abstract class DomainRepository : DisposableObject, IDomainRepository
    {
        private readonly Guid id = Guid.NewGuid();

        public Guid Id => this.id;

        public abstract TAggregateRoot GetById<TKey, TAggregateRoot>(TKey id)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRootWithEventSourcing<TKey>;

        public Task<TAggregateRoot> GetByIdAsync<TKey, TAggregateRoot>(TKey id, CancellationToken cancellationToken)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRootWithEventSourcing<TKey> => Task.FromResult(this.GetById<TKey, TAggregateRoot>(id));

        public abstract void Save<TKey, TAggregateRoot>(TAggregateRoot aggregateRoot)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRootWithEventSourcing<TKey>;

        public Task SaveAsync<TKey, TAggregateRoot>(TAggregateRoot aggregateRoot, CancellationToken cancellationToken)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRootWithEventSourcing<TKey>
            => Task.Factory.StartNew(() => Save<TKey, TAggregateRoot>(aggregateRoot), cancellationToken);
    }
}