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

using Apworks.Repositories;
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

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceController{TKey, TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="repositoryContext">The repository context.</param>
        protected DataServiceController(IRepositoryContext repositoryContext)
        {
            this.repositoryContext = repositoryContext;
            this.repository = repositoryContext.GetRepository<TKey, TAggregateRoot>();
        }
        #endregion

        protected IRepositoryContext RepositoryContext => this.repositoryContext;

        protected IRepository<TKey, TAggregateRoot> Repository => this.repository;

        [HttpGet]
        public virtual async Task<IQueryable<TAggregateRoot>> Get()
        {
            return await this.repository.FindAllAsync();
        }

        [HttpGet("{id}")]
        public virtual TAggregateRoot Get(TKey id)
        {
            return this.repository.FindByKey(id);
        }

        [HttpPost]
        public virtual async Task Post([FromBody] TAggregateRoot aggregateRoot)
        {
            await this.repository.AddAsync(aggregateRoot);
        }

        [HttpPut("{id}")]
        public virtual async Task Put(TKey id, [FromBody] TAggregateRoot aggregateRoot)
        {
            if (!aggregateRoot.Id.Equals(id))
            {
                throw new InvalidOperationException();
            }
            await this.repository.UpdateAsync(aggregateRoot);
        }
    }
}
