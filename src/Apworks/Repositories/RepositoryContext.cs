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

using System;
using System.Collections.Concurrent;
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
        #region Private Fields
        private readonly TSession session;
        private readonly Guid id = Guid.NewGuid();
        private readonly ConcurrentDictionary<Type, object> cachedRepositories = new ConcurrentDictionary<Type, object>();
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryContext{TSession}"/> class.
        /// </summary>
        /// <param name="session">The encapsulated session object.</param>
        protected RepositoryContext(TSession session)
        {
            this.session = session;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the unique identifier of the current repository context.
        /// </summary>
        public Guid Id => this.id;

        /// <summary>
        /// Gets the instance of a session object.
        /// </summary>
        /// <remarks>
        /// A session object usually maintains the connection between the repository and
        /// its backend infrastructure. For example, it can be the DbContext in Entity Framework
        /// repository implementation, or an ISession instance in NHibernate implementation.
        /// </remarks>
        public TSession Session => this.session;

        /// <summary>
        /// Gets the instance of a session object.
        /// </summary>
        /// <remarks>
        /// A session object usually maintains the connection between the repository and
        /// its backend infrastructure. For example, it can be the DbContext in Entity Framework
        /// repository implementation, or an ISession instance in NHibernate implementation.
        /// </remarks>
        object IRepositoryContext.Session => this.session;
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets an instance of <see cref="T:Apworks.Repositories.IRepository`2" /> from current context.
        /// </summary>
        /// <typeparam name="TKey">The type of the key of the aggregate root.</typeparam>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
        /// <returns>
        /// An instance of <see cref="T:Apworks.Repositories.IRepository`2" />.
        /// </returns>
        public IRepository<TKey, TAggregateRoot> GetRepository<TKey, TAggregateRoot>()
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRoot<TKey> =>
            (IRepository<TKey, TAggregateRoot>)cachedRepositories.GetOrAdd(typeof(TAggregateRoot), CreateRepository<TKey, TAggregateRoot>());

        /// <summary>
        /// Commits the changes to the repository as a single transaction.
        /// </summary>
        public virtual void Commit() { }

        /// <summary>
        /// Commits the changes to the repository asynchronously as a single transaction.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> that propagates the notification that the operation should be cancelled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> which performs the commit operation.
        /// </returns>
        public virtual Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Factory.StartNew(Commit, cancellationToken);
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Creates the repository.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
        /// <returns></returns>
        protected abstract IRepository<TKey, TAggregateRoot> CreateRepository<TKey, TAggregateRoot>()
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRoot<TKey>;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing) { }
        #endregion

    }
}
