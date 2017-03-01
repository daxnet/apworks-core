using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Apworks.Querying;

namespace Apworks.Repositories.Dictionary
{
    internal sealed class DictionaryRepository<TKey, TAggregateRoot> : Repository<TKey, TAggregateRoot>
        where TKey : IEquatable<TKey>
        where TAggregateRoot : class, IAggregateRoot<TKey>
    {
        private readonly DictionaryRepositoryContext context;

        public DictionaryRepository(IRepositoryContext context) : base(context)
        {
            this.context = (DictionaryRepositoryContext)context;
        }

        public override void Add(TAggregateRoot aggregateRoot)
        {
            this.context.Session.TryAdd(aggregateRoot.Id, aggregateRoot);
        }

        public override IQueryable<TAggregateRoot> FindAll(Expression<Func<TAggregateRoot, bool>> specification, SortSpecification<TKey, TAggregateRoot> sortSpecification)
        {
            throw new NotImplementedException();
        }

        public override PagedResult<TKey, TAggregateRoot> FindAll(Expression<Func<TAggregateRoot, bool>> specification, SortSpecification<TKey, TAggregateRoot> sortSpecification, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public override TAggregateRoot FindByKey(TKey key)
        {
            object result = null;
            if (this.context.Session.TryGetValue(key, out result))
            {
                return (TAggregateRoot)result;
            }

            return null;
        }

        public override void RemoveByKey(TKey key)
        {
            object result = null;
            this.context.Session.TryRemove(key, out result);
        }

        public override void UpdateByKey(TKey key, TAggregateRoot aggregateRoot)
        {

        }
    }
}
