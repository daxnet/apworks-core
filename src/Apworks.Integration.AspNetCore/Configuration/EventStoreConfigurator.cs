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

using Apworks.Events;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Apworks.Integration.AspNetCore.Configuration
{
    ///// <summary>
    ///// Represents that the implemented classes are event store configurators.
    ///// </summary>
    ///// <seealso cref="Apworks.Integration.AspNetCore.Configuration.IConfigurator" />
    //internal interface IEventStoreConfigurator : IConfigurator
    //{ }

    ///// <summary>
    ///// The default implementation of the <see cref="IEventStoreConfigurator"/> interface.
    ///// </summary>
    ///// <typeparam name="TEventStore">The type of the event store.</typeparam>
    ///// <seealso cref="Apworks.Integration.AspNetCore.Configuration.ServiceRegisterConfigurator{Apworks.Events.IEventStore, TEventStore}" />
    ///// <seealso cref="Apworks.Integration.AspNetCore.Configuration.IEventStoreConfigurator" />
    //internal sealed class EventStoreConfigurator<TEventStore> : ServiceRegisterConfigurator<IEventStore, TEventStore>, IEventStoreConfigurator
    //    where TEventStore : class, IEventStore
    //{
    //    #region Ctor
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="EventStoreConfigurator{TEventStore}"/> class.
    //    /// </summary>
    //    /// <param name="context">The context that can register the specified service implementations against their service contracts
    //    /// to the <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> instance.</param>
    //    /// <param name="implementation">The implementation of the service that is going to be registered.</param>
    //    /// <param name="serviceLifetime">The service lifetime.</param>
    //    public EventStoreConfigurator(IConfigurator context, TEventStore implementation, ServiceLifetime serviceLifetime) : base(context, implementation, serviceLifetime)
    //    {
    //    }

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="EventStoreConfigurator{TEventStore}"/> class.
    //    /// </summary>
    //    /// <param name="context">The context that can register the specified service implementations against their service contracts
    //    /// to the <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> instance.</param>
    //    /// <param name="implementationFactory">The implementation factory that creates the implementation of the service that is going
    //    /// to be registered.</param>
    //    /// <param name="serviceLifetime">The service lifetime.</param>
    //    public EventStoreConfigurator(IConfigurator context, Func<IServiceProvider, TEventStore> implementationFactory, ServiceLifetime serviceLifetime) : base(context, implementationFactory, serviceLifetime)
    //    {
    //    }
    //    #endregion
    //}
}
