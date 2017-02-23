// ==================================================================================================================                                                                                          
//        ,::i                                                           BBB                
//       BBBBBi                                                         EBBB                
//      MBBNBBU                                                         BBB,                
//     BBB. BBB     BBB,BBBBM   BBB   UBBB   MBB,  LBBBBBO,   :BBG,BBB :BBB  .BBBU  kBBBBBF 
//    BBB,  BBB    7BBBBS2BBBO  BBB  iBBBB  YBBJ :BBBMYNBBB:  FBBBBBB: OBB: 5BBB,  BBBi ,M, 
//   MBBY   BBB.   8BBB   :BBB  BBB .BBUBB  BB1  BBBi   kBBB  BBBM     BBBjBBBr    BBB1     
//  BBBBBBBBBBBu   BBB    FBBP  MBM BB. BB BBM  7BBB    MBBY .BBB     7BBGkBB1      JBBBBi  
// PBBBFE0GkBBBB  7BBX   uBBB   MBBMBu .BBOBB   rBBB   kBBB  ZBBq     BBB: BBBJ   .   iBBB  
//BBBB      iBBB  BBBBBBBBBE    EBBBB  ,BBBB     MBBBBBBBM   BBB,    iBBB  .BBB2 :BBBBBBB7  
//vr7        777  BBBu8O5:      .77r    Lr7       .7EZk;     L77     .Y7r   irLY  JNMMF:    
//               LBBj
//
// Apworks Application Development Framework
// Copyright (C) 2009-2017 by daxnet.
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ==================================================================================================================

using Apworks.KeyGeneration;
using Apworks.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.DataServices
{
    /// <summary>
    /// Represents the base class for the controllers that provide the simple
    /// data services against the aggregate root with a particular type.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public abstract class DataServiceController<TKey, TAggregateRoot> : Controller
        where TAggregateRoot : class, IAggregateRoot<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly IRepositoryContext repositoryContext;
        private readonly IRepository<TKey, TAggregateRoot> repository;
        private readonly IKeyGenerator<TKey, TAggregateRoot> keyGenerator;

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceController{TKey, TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="repositoryContext">The repository context that is used by the current
        /// <see cref="DataServiceController{TKey, TAggregateRoot}"/> for managing the object lifecycle.</param>
        protected DataServiceController(IRepositoryContext repositoryContext)
            : this(repositoryContext, new NullKeyGenerator<TKey>())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceController{TKey, TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="repositoryContext">The repository context that is used by the current
        /// <see cref="DataServiceController{TKey, TAggregateRoot}"/> for managing the object lifecycle.</param>
        /// <param name="keyGenerator">The <see cref="IKeyGenerator{TKey, TAggregateRoot}"/> instance
        /// which generates the aggregate root key for the specified aggregate root type.</param>
        protected DataServiceController(IRepositoryContext repositoryContext, IKeyGenerator<TKey, TAggregateRoot> keyGenerator)
        {
            this.repositoryContext = repositoryContext;
            this.repository = repositoryContext.GetRepository<TKey, TAggregateRoot>();
            this.keyGenerator = keyGenerator;
        }
        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the repository context that is used by the current
        /// <see cref="DataServiceController{TKey, TAggregateRoot}"/> for managing the object lifecycle.
        /// </summary>
        protected IRepositoryContext RepositoryContext => this.repositoryContext;

        protected IRepository<TKey, TAggregateRoot> Repository => this.repository;

        protected IKeyGenerator<TKey, TAggregateRoot> KeyGenerator => this.keyGenerator;
        #endregion

        [HttpGet]
        public virtual async Task<IQueryable<TAggregateRoot>> Get()
        {
            return await this.repository.FindAllAsync();
        }

        [HttpGet("{id}")]
        public virtual async Task<TAggregateRoot> Get(TKey id)
        {
            if (id.Equals(default(TKey)))
            {
                throw new InvalidArgumentException("Entity key has not been specified.");
            }

            var all = await this.repository.FindAllAsync(x => x.Id.Equals(id));
            var first = all.FirstOrDefault();
            if (first == null)
            {
                throw new EntityNotFoundException($"The entity with the key of '{id}' does not exist.");
            }

            return first;
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] TAggregateRoot aggregateRoot)
        {
            if (aggregateRoot == null)
            {
                throw new InvalidOperationException("The entity that is going to be created has not been specified.");
            }

            if (!aggregateRoot.Id.Equals(default(TKey)) &&
                await this.repository.ExistsAsync(x => x.Id.Equals(aggregateRoot.Id)))
            {
                throw new EntityAlreadyExistsException($"The entity with the key of '{aggregateRoot.Id}' already exists.");
            }

            var generatedKey = this.keyGenerator.Generate(aggregateRoot);
            if (!generatedKey.Equals(default(TKey)))
            {
                aggregateRoot.Id = generatedKey;
            }

            await this.repository.AddAsync(aggregateRoot);
            await this.repositoryContext.CommitAsync();

            return Created(Url.Action("Get", new { id = aggregateRoot.Id }), aggregateRoot.Id);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Put(TKey id, [FromBody] TAggregateRoot aggregateRoot)
        {
            if (id.Equals(default(TKey)))
            {
                throw new InvalidArgumentException("Entity key has not been specified.");
            }

            if (aggregateRoot == null)
            {
                throw new InvalidArgumentException("The entity that is going to be updated has not been specified.");
            }

            if (!(await this.repository.ExistsAsync(x => x.Id.Equals(id))))
            {
                throw new EntityNotFoundException($"The entity with the key of '{id}' does not exist.");
            }

            await this.repository.UpdateByKeyAsync(id, aggregateRoot);
            await this.repositoryContext.CommitAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(TKey id)
        {
            if (id.Equals(default(TKey)))
            {
                throw new InvalidArgumentException("Entity key has not been specified.");
            }

            if (!await this.repository.ExistsAsync(x => x.Id.Equals(id)))
            {
                throw new EntityNotFoundException($"The entity with the key of '{id}' does not exist.");
            }

            await this.repository.RemoveByKeyAsync(id);
            await this.repositoryContext.CommitAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public virtual async Task<IActionResult> Patch(TKey id, [FromBody] JsonPatchDocument<TAggregateRoot> patch)
        {
            if (id.Equals(default(TKey)))
            {
                throw new InvalidArgumentException("Entity key has not been specified.");
            }

            var instance = (await this.repository.FindAllAsync(x => x.Id.Equals(id))).FirstOrDefault();
            if (instance == null)
            {
                throw new EntityNotFoundException($"The entity with the key of '{id}' does not exist.");
            }

            patch.ApplyTo(instance, this.ModelState);

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(this.ModelState);
            }

            await this.repository.UpdateByKeyAsync(id, instance);
            await this.repositoryContext.CommitAsync();

            return NoContent();
        }
    }
}
