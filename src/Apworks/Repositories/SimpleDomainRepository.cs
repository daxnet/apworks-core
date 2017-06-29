using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Repositories
{
    public sealed class SimpleDomainRepository : DomainRepository
    {
        private readonly IRepositoryContext repositoryContext;

        public SimpleDomainRepository(IRepositoryContext repositoryContext)
        {
            this.repositoryContext = repositoryContext;
        }

        public override TAggregateRoot GetById<TKey, TAggregateRoot>(TKey id)
        {
            var repository = this.repositoryContext.GetRepository<TKey, TAggregateRoot>();
            return repository.FindByKey(id);
        }

        public override TAggregateRoot GetById<TKey, TAggregateRoot>(TKey id, long version) => this.GetById<TKey, TAggregateRoot>(id);
        

        public override async Task<TAggregateRoot> GetByIdAsync<TKey, TAggregateRoot>(TKey id, CancellationToken cancellationToken)
        {
            var repository = this.repositoryContext.GetRepository<TKey, TAggregateRoot>();
            return await repository.FindByKeyAsync(id, cancellationToken);
        }

        public override async Task<TAggregateRoot> GetByIdAsync<TKey, TAggregateRoot>(TKey id, long version, CancellationToken cancellationToken)
            => await this.GetByIdAsync<TKey, TAggregateRoot>(id, cancellationToken);

        public override void Save<TKey, TAggregateRoot>(TAggregateRoot aggregateRoot)
        {
            var repository = this.repositoryContext.GetRepository<TKey, TAggregateRoot>();
            repository.Add(aggregateRoot);
            this.repositoryContext.Commit();
        }

        public override async Task SaveAsync<TKey, TAggregateRoot>(TAggregateRoot aggregateRoot, CancellationToken cancellationToken)
        {
            var repository = this.repositoryContext.GetRepository<TKey, TAggregateRoot>();
            await repository.AddAsync(aggregateRoot);
            await this.repositoryContext.CommitAsync();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.repositoryContext?.Dispose();
            }
        }

        
    }
}
