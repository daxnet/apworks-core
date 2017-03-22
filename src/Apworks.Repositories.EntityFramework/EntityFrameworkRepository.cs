using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apworks.Querying;
using Microsoft.EntityFrameworkCore;

namespace Apworks.Repositories.EntityFramework
{
    internal sealed class EntityFrameworkRepository<TKey, TAggregateRoot> : Repository<TKey, TAggregateRoot>
        where TKey : IEquatable<TKey>
        where TAggregateRoot : class, IAggregateRoot<TKey>
    {
        private readonly DbContext dbContext;

        public EntityFrameworkRepository(IRepositoryContext context) : base(context)
        {
            var entityFrameworkRepositoryContext = (EntityFrameworkRepositoryContext)context;
            this.dbContext = entityFrameworkRepositoryContext.Session;
        }

        public override void Add(TAggregateRoot aggregateRoot)
        {
            this.dbContext
                .Set<TAggregateRoot>()
                .Add(aggregateRoot);
        }

        public override async Task AddAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken = default(CancellationToken))
        {
            await this.dbContext
                .Set<TAggregateRoot>()
                .AddAsync(aggregateRoot, cancellationToken);
        }

        public override IEnumerable<TAggregateRoot> FindAll(Expression<Func<TAggregateRoot, bool>> specification, SortSpecification<TKey, TAggregateRoot> sortSpecification)
        {
            return this.dbContext
                .Set<TAggregateRoot>()
                .Where(specification);
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
