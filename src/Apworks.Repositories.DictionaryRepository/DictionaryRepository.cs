using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Apworks.Querying;

namespace Apworks.Repositories.DictionaryRepository
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override void RemoveByKey(TKey key)
        {
            throw new NotImplementedException();
        }

        public override void UpdateByKey(TKey key, TAggregateRoot aggregateRoot)
        {
            throw new NotImplementedException();
        }
    }
}
