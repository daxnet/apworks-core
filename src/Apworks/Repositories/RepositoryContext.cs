using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Repositories
{
    public abstract class RepositoryContext<TSession> : DisposableObject, IRepositoryContext<TSession>
        where TSession : class
    {
        private readonly TSession session;
        private readonly Guid id = Guid.NewGuid();
        private readonly ConcurrentDictionary<Type, object> cachedRepositories = new ConcurrentDictionary<Type, object>();

        protected RepositoryContext(TSession session)
        {
            this.session = session;
        }

        public Guid Id => this.id;

        public TSession Session => this.session;

        object IRepositoryContext.Session => this.session;

        public IRepository<TKey, TAggregateRoot> GetRepository<TKey, TAggregateRoot>()
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRoot<TKey> =>
            (IRepository<TKey, TAggregateRoot>)cachedRepositories.GetOrAdd(typeof(TAggregateRoot), CreateRepository<TKey, TAggregateRoot>());

        protected abstract IRepository<TKey, TAggregateRoot> CreateRepository<TKey, TAggregateRoot>()
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRoot<TKey>;

        protected override void Dispose(bool disposing) { }

        public virtual void Commit()
        {
            
        }

        public virtual Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Factory.StartNew(Commit, cancellationToken);
        }
    }
}
