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

using Apworks.KeyGeneration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Apworks.Integration.AspNetCore.Configuration
{
    /// <summary>
    /// Represents that the implemented classes are the configurators that can register
    /// an <see cref="IKeyGenerator{TKey, TAggregateRoot}"/> instance to the <see cref="IServiceCollection"/> instance.
    /// </summary>
    /// <seealso cref="Apworks.Integration.AspNetCore.Configuration.IConfigurator" />
    public interface IKeyGeneratorConfigurator : IConfigurator
    {

    }

    /// <summary>
    /// Represents the default implementation of <see cref="IKeyGenerator{TKey, TAggregateRoot}"/> interface.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
    /// <typeparam name="TKeyGenerator">The type of the key generator.</typeparam>
    /// <seealso cref="Apworks.Integration.AspNetCore.Configuration.ServiceRegisterConfigurator{Apworks.KeyGeneration.IKeyGenerator{TKey, TAggregateRoot}, TKeyGenerator}" />
    /// <seealso cref="Apworks.Integration.AspNetCore.Configuration.IKeyGeneratorConfigurator" />
    internal sealed class KeyGeneratorConfigurator<TKey, TAggregateRoot, TKeyGenerator> 
        : ServiceRegisterConfigurator<IKeyGenerator<TKey, TAggregateRoot>, TKeyGenerator>, IKeyGeneratorConfigurator
        where TKey : IEquatable<TKey>
        where TAggregateRoot : class, IAggregateRoot<TKey>
        where TKeyGenerator : class, IKeyGenerator<TKey, TAggregateRoot>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyGeneratorConfigurator{TKey, TAggregateRoot, TKeyGenerator}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="implementation">The implementation.</param>
        /// <param name="serviceLifetime">The service lifetime.</param>
        public KeyGeneratorConfigurator(IConfigurator context, TKeyGenerator implementation, ServiceLifetime serviceLifetime)
            : base(context, implementation, serviceLifetime)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyGeneratorConfigurator{TKey, TAggregateRoot, TKeyGenerator}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="implementationFactory">The implementation factory.</param>
        /// <param name="serviceLifetime">The service lifetime.</param>
        public KeyGeneratorConfigurator(IConfigurator context, Func<IServiceProvider, TKeyGenerator> implementationFactory, ServiceLifetime serviceLifetime)
            : base(context, implementationFactory, serviceLifetime)
        { }
    }
}
