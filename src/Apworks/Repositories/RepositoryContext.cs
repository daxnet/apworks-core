using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Repositories
{
    /// <summary>
    /// Represents the base class for the repository contexts.
    /// </summary>
    /// <remarks>
    /// A repository context is the concept that handles external storage connection sessions and
    /// database transactions. Usually the encapsulated session object will be a database connection,
    /// a database session that holds the database connection and handles the object states, or even
    /// a centralized location where the object data stores. For example, a DbContext object in Entity Framework,
    /// a Session object in NHibernate or an IDbConnection object in ADO.NET.
    /// </remarks>
    /// <typeparam name="TSession">The type of the encapsulated session object.</typeparam>
    public abstract class RepositoryContext<TSession> : DisposableObject, IRepositoryContext<TSession>
        where TSession : class
    {
        private readonly TSession session;
        private readonly Guid id = Guid.NewGuid();
        private readonly ConcurrentDictionary<Type, object> cachedRepositories = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryContext{TSession}"/> class.
        /// </summary>
        /// <param name="session">The encapsulated session object.</param>
        protected RepositoryContext(TSession session)
        {
            this.session = session;
        }

        /// <summary>
        /// Gets the unique identifier of the current repository context.
        /// </summary>
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
