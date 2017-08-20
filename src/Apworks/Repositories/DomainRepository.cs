using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public abstract TAggregateRoot GetById<TKey, TAggregateRoot>(TKey id, long version)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRootWithEventSourcing<TKey>;

        public virtual Task<TAggregateRoot> GetByIdAsync<TKey, TAggregateRoot>(TKey id, CancellationToken cancellationToken)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRootWithEventSourcing<TKey> => Task.FromResult(this.GetById<TKey, TAggregateRoot>(id));

        public virtual Task<TAggregateRoot> GetByIdAsync<TKey, TAggregateRoot>(TKey id, long version, CancellationToken cancellationToken)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRootWithEventSourcing<TKey> => Task.FromResult(this.GetById<TKey, TAggregateRoot>(id, version));

        public abstract void Save<TKey, TAggregateRoot>(TAggregateRoot aggregateRoot)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRootWithEventSourcing<TKey>;

        public virtual Task SaveAsync<TKey, TAggregateRoot>(TAggregateRoot aggregateRoot, CancellationToken cancellationToken)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRootWithEventSourcing<TKey>
            => Task.Factory.StartNew(() => Save<TKey, TAggregateRoot>(aggregateRoot), cancellationToken);

        /// <summary>
        /// Activates an aggregate root with the specified type.
        /// </summary>
        /// <typeparam name="TARKey">The type of the aggregate root id.</typeparam>
        /// <typeparam name="TAR">The type of the aggregate root.</typeparam>
        /// <returns>The activated aggregate root instance.</returns>
        protected TAR ActivateAggregateRoot<TARKey, TAR>()
            where TARKey : IEquatable<TARKey>
            where TAR : class, IAggregateRootWithEventSourcing<TARKey>
        {
            var aggregateRootType = typeof(TAR);
            var constructorInfoQuery = from ctor in aggregateRootType.GetTypeInfo().GetConstructors()
                                       let parameters = ctor.GetParameters()
                                       where parameters.Length == 1 && parameters[0].ParameterType == typeof(TARKey)
                                       select ctor;
            var constructorInfo = constructorInfoQuery.FirstOrDefault();
            if (constructorInfo != null)
            {
                var result = (TAR)constructorInfo.Invoke(new object[] { default(TARKey) });
                ((IPurgeable)result).Purge();
                // Note: Violates Liskov Substitution Principle
                (result as AggregateRootWithEventSourcing<TARKey>).PersistedVersion = 0;
                return result;
            }

            return null;
        }
    }
}