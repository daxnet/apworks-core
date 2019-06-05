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
// Copyright (C) 2009-2018 by daxnet.
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

using Apworks.Integration.AspNetCore.Hal;
using Apworks.KeyGeneration;
using Apworks.Querying;
using Apworks.Querying.Parsers.Irony;
using Apworks.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Expressions;
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
    [SupportsHal]
    public abstract class DataServiceController<TKey, TAggregateRoot> : ControllerBase
        where TAggregateRoot : class, IAggregateRoot<TKey>
        where TKey : IEquatable<TKey>
    {
        #region Fields
        private readonly IRepositoryContext repositoryContext;
        private readonly IRepository<TKey, TAggregateRoot> repository;
        private readonly IQueryConditionParser queryConditionParser;
        private readonly ISortSpecificationParser sortSpecificationParser;
        private readonly IKeyGenerator<TKey, TAggregateRoot> keyGenerator;
        //private bool disposed = false;
        #endregion

        #region Ctor        
        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceController{TKey, TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="repositoryContext">The repository context that is used for initializing the data service controller.</param>
        public DataServiceController(IRepositoryContext repositoryContext)
            : this(repositoryContext, new NullKeyGenerator<TKey>(), new IronyQueryConditionParser(), new IronySortSpecificationParser())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceController{TKey, TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="repositoryContext">The repository context that is used for initializing the data service controller.</param>
        /// <param name="keyGenerator">The key generator which is used for generating the aggregate root key.
        /// If the persistence mechanism will generate the key automatically, for example, in SQL Server databases, an auto increment
        /// value is used for the key column, the <see cref="NullKeyGenerator{TKey}"/> can be used for this parameter.
        /// </param>
        public DataServiceController(IRepositoryContext repositoryContext, IKeyGenerator<TKey, TAggregateRoot> keyGenerator)
            : this(repositoryContext, keyGenerator, new IronyQueryConditionParser(), new IronySortSpecificationParser())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceController{TKey, TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="repositoryContext">The repository context that is used for initializing the data service controller.</param>
        /// <param name="queryConditionParser">The query condition parser which parses a given query string into a lambda expression.</param>
        /// <param name="sortSpecificationParser">The sort specification parser which parses a given sort specification string
        /// into a lambda expression.</param>
        public DataServiceController(IRepositoryContext repositoryContext, IQueryConditionParser queryConditionParser, ISortSpecificationParser sortSpecificationParser)
            : this(repositoryContext, new NullKeyGenerator<TKey>(), queryConditionParser, sortSpecificationParser)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceController{TKey, TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="repositoryContext">The repository context that is used for initializing the data service controller.</param>
        /// <param name="keyGenerator">The key generator which is used for generating the aggregate root key.
        /// If the persistence mechanism will generate the key automatically, for example, in SQL Server databases, an auto increment
        /// value is used for the key column, the <see cref="NullKeyGenerator{TKey}"/> can be used for this parameter.</param>
        /// <param name="queryConditionParser">The query condition parser which parses a given query string into a lambda expression.</param>
        /// <param name="sortSpecificationParser">The sort specification parser which parses a given sort specification string
        /// into a lambda expression.</param>
        public DataServiceController(IRepositoryContext repositoryContext, 
            IKeyGenerator<TKey, TAggregateRoot> keyGenerator,
            IQueryConditionParser queryConditionParser,
            ISortSpecificationParser sortSpecificationParser)
        {
            this.repositoryContext = repositoryContext;
            this.repository = repositoryContext.GetRepository<TKey, TAggregateRoot>();
            this.keyGenerator = keyGenerator;
            this.queryConditionParser = queryConditionParser;
            this.sortSpecificationParser = sortSpecificationParser;
        }
        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the repository context instance which orchestrates the data access operations.
        /// </summary>
        /// <value>
        /// The repository context.
        /// </value>
        protected IRepositoryContext RepositoryContext => this.repositoryContext;

        /// <summary>
        /// Gets the repository which manages aggregates' life cycle.
        /// </summary>
        /// <value>
        /// The repository.
        /// </value>
        protected IRepository<TKey, TAggregateRoot> Repository => this.repository;

        /// <summary>
        /// Gets the key generator which is used for generating the aggregate root key.
        /// </summary>
        /// <value>
        /// The key generator.
        /// </value>
        protected IKeyGenerator<TKey, TAggregateRoot> KeyGenerator => this.keyGenerator;

        /// <summary>
        /// Gets the query condition parser which parses a given query string into a lambda expression.
        /// </summary>
        /// <value>
        /// The query condition parser.
        /// </value>
        protected IQueryConditionParser QueryConditionParser => this.queryConditionParser;

        /// <summary>
        /// Gets the sort specification parser which parses a given sort specification string
        /// into a lambda expression.
        /// </summary>
        /// <value>
        /// The sort specification parser.
        /// </value>
        protected ISortSpecificationParser SortSpecificationParser => this.sortSpecificationParser;

        #endregion

        /// <summary>
        /// Returns all the aggregates that match specific query criteria in a specified order, with pagination support.
        /// </summary>
        /// <param name="size">The number of aggregates that will be contained in a page (the page size).</param>
        /// <param name="page">The page number.</param>
        /// <param name="query">The string which specifies the query criteria.</param>
        /// <param name="sort">The string which specifies the ordering specification.</param>
        /// <returns>A list of the aggregates that match specific query criteria in a specified order.</returns>
        [HttpGet]
        public virtual async Task<IActionResult> Get([FromQuery] int size = 15, 
            [FromQuery] int page = 1,
            [FromQuery] string query = "",
            [FromQuery] string sort = "")
        {
            try
            {
                Expression<Func<TAggregateRoot, bool>> queryCondition = _ => true;
                if (!string.IsNullOrEmpty(query))
                {
                    queryCondition = this.queryConditionParser.Parse<TAggregateRoot>(query);
                }

                SortSpecification<TKey, TAggregateRoot> sortSpecification = new SortSpecification<TKey, TAggregateRoot> { { x => x.Id, SortOrder.Ascending } };
                if (!string.IsNullOrEmpty(sort))
                {
                    sortSpecification = this.sortSpecificationParser.Parse<TKey, TAggregateRoot>(sort);
                }

                var aggregateRoots = await this.repository.FindAllAsync(queryCondition, sortSpecification, page, size);

                return Ok(aggregateRoots);
            }
            catch (ParsingException pe)
            {
                var parsingErrors = string.Join(Environment.NewLine, pe.ParseErrors);
                throw new InvalidArgumentException($"The specified query or sort is invalid. Details: {Environment.NewLine}{parsingErrors}", pe);
            }
        }

        /// <summary>
        /// Gets an aggregate with the specified aggregate root key (the aggregate Id).
        /// </summary>
        /// <param name="id">The key of the aggregate root.</param>
        /// <returns>The aggregate whose Id equals to the specified key.</returns>
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

        /// <summary>
        /// Adds the aggregate to the repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root of the aggregate that is going to be added.</param>
        /// <returns>HTTP status 201 (Created) with the generated Id and resource URI.</returns>
        /// <exception cref="InvalidOperationException">The entity that is going to be created has not been specified.</exception>
        /// <exception cref="EntityAlreadyExistsException">The entity of the given key in the POST body already exists.</exception>
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

        /// <summary>
        /// Modifies the aggregate that has the specified key, by using the specified data.
        /// </summary>
        /// <param name="id">The key of the aggregate root.</param>
        /// <param name="aggregateRoot">The aggregate root which contains the data for updating an existing aggregate.</param>
        /// <returns>HTTP 204 (No content), if the update is successful.</returns>
        /// <exception cref="InvalidArgumentException">
        /// Entity key has not been specified.
        /// or
        /// The entity that is going to be updated has not been specified.
        /// </exception>
        /// <exception cref="EntityNotFoundException">Throws when the aggregate with the specified aggregate root key does not exist.</exception>
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

        /// <summary>
        /// Deletes the aggregate with the specified key.
        /// </summary>
        /// <param name="id">The key of the aggregate root.</param>
        /// <returns>HTTP 204 (No content) if deletion is successful.</returns>
        /// <exception cref="InvalidArgumentException">Entity key has not been specified.</exception>
        /// <exception cref="EntityNotFoundException">Throws when the entity has not been found.</exception>
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

        /// <summary>
        /// Updates the aggregate that has the specified aggregate root key by using the specified aggregate data and update path.
        /// </summary>
        /// <param name="id">The key of the aggregate root that is going to be updated.</param>
        /// <param name="patch">The <see cref="JsonPatchDocument{TModel}"/> instance that defines the update path.</param>
        /// <returns></returns>
        /// <exception cref="InvalidArgumentException">Entity key has not been specified.</exception>
        /// <exception cref="EntityNotFoundException">Throws when the entity has not been found.</exception>
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

            patch.ApplyTo(instance);

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(this.ModelState);
            }

            await this.repository.UpdateByKeyAsync(id, instance);
            await this.repositoryContext.CommitAsync();

            return NoContent();
        }

        ///// <summary>
        ///// Releases all resources currently used by this <see cref="T:Microsoft.AspNetCore.Mvc.Controller" /> instance.
        ///// </summary>
        ///// <param name="disposing"><c>true</c> if this method is being invoked by the <see cref="M:Microsoft.AspNetCore.Mvc.Controller.Dispose" /> method,
        ///// otherwise <c>false</c>.</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (!disposed)
        //    {
        //        if (disposing)
        //        {
        //            this.repositoryContext.Dispose();
        //        }

        //        disposed = true;
        //        base.Dispose(disposing);
        //    }
        //}
    }
}
