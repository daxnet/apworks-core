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

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Repositories
{
    /// <summary>
    /// Represents that the implemented classes are repository contexts in which
    /// the repositories resides.
    /// </summary>
    public interface IRepositoryContext : IDisposable
    {
        /// <summary>
        /// Gets the unique identifier of the current context.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the instance of a session object.
        /// </summary>
        /// <remarks>
        /// A session object usually maintains the connection between the repository and 
        /// its backend infrastructure. For example, it can be the DbContext in Entity Framework
        /// repository implementation, or an ISession instance in NHibernate implementation.
        /// </remarks>
        object Session { get; }

        /// <summary>
        /// Gets an instance of <see cref="IRepository{TKey, TAggregateRoot}"/> from current context.
        /// </summary>
        /// <typeparam name="TKey">The type of the key of the aggregate root.</typeparam>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
        /// <returns>An instance of <see cref="IRepository{TKey, TAggregateRoot}"/>.</returns>
        IRepository<TKey, TAggregateRoot> GetRepository<TKey, TAggregateRoot>()
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRoot<TKey>;

        /// <summary>
        /// Commits the changes to the repository as a single transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Commits the changes to the repository asynchronously as a single transaction.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that propagates the notification that the operation should be cancelled.</param>
        /// <returns>The <see cref="Task"/> which performs the commit operation.</returns>
        Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Represents that the implemented classes are repository contexts in which
    /// the repositories resides. This is the strongly-typed version of <see cref="IRepositoryContext"/> interface.
    /// </summary>
    /// <typeparam name="TSession">The type of the session object.</typeparam>
    public interface IRepositoryContext<out TSession> : IRepositoryContext
        where TSession : class
    {
        /// <summary>
        /// Gets the instance of a session object.
        /// </summary>
        /// <remarks>
        /// A session object usually maintains the connection between the repository and 
        /// its backend infrastructure. For example, it can be the DbContext in Entity Framework
        /// repository implementation, or an ISession instance in NHibernate implementation.
        /// </remarks>
        new TSession Session { get; }
    }
}
