﻿// ==================================================================================================================                                                                                          
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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Apworks.Utilities
{
    /// <summary>
    /// Represents the class which contains the utility methods.
    /// </summary>
    public static class Utils
    {
        private const int InitialPrime = 23;
        private const int FactorPrime = 29;

        private static Lazy<Type[]> SimpleTypesInternal = new Lazy<Type[]>(() =>
        {
            var types = new[]
                           {
                              typeof (Enum),
                              typeof (string),
                              typeof (char),
                              typeof (Guid),

                              typeof (bool),
                              typeof (byte),
                              typeof (short),
                              typeof (int),
                              typeof (long),
                              typeof (float),
                              typeof (double),
                              typeof (decimal),

                              typeof (sbyte),
                              typeof (ushort),
                              typeof (uint),
                              typeof (ulong),

                              typeof (DateTime),
                              typeof (DateTimeOffset),
                              typeof (TimeSpan),
                          };


            var nullableTypes = from t in types
                                where t != typeof(Enum) && t != typeof(string)
                                select typeof(Nullable<>).MakeGenericType(t);

            return types.Concat(nullableTypes).ToArray();
        });

        /// <summary>
        /// Determines whether the current <see cref="Type"/> is a simple CLR type.
        /// </summary>
        /// <param name="src">The current CLR type.</param>
        /// <returns>
        ///   <c>true</c> if the current <see cref="Type"/> is a simple CLR type; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">src</exception>
        public static bool IsSimpleType(this Type src)
        {
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            return src.GetTypeInfo().IsEnum ||
                (src.GetTypeInfo().IsGenericType &&
                    src.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>) &&
                    src.GetTypeInfo().GetGenericArguments().First().GetTypeInfo().IsEnum) ||
                SimpleTypesInternal.Value.Contains(src);
        }

        /// <summary>
        /// Pluralizes the specified word.
        /// </summary>
        /// <param name="word">The word to be pluralized.</param>
        /// <param name="inputIsKnownToBeSingular">True if the caller can ensure that the word
        /// passed in is in the singular form. Otherwise, false.</param>
        /// <returns>The pluralized word.</returns>
        /// <remarks>
        /// This code is from the Humanizer open source library: https://github.com/Humanizr/Humanizer.
        /// </remarks>
        public static string Pluralize(this string word, bool inputIsKnownToBeSingular = true)
        {
            return Vocabularies.Default.Pluralize(word, inputIsKnownToBeSingular);
        }

        /// <summary>
        /// Gets the hash code for an object based on the given array of hash
        /// codes from each property of the object.
        /// </summary>
        /// <param name="hashCodesForProperties">The array of the hash codes
        /// that are from each property of the object.</param>
        /// <returns>The hash code.</returns>
        public static int GetHashCode(params int[] hashCodesForProperties)
        {
            unchecked
            {
                int hash = InitialPrime;
                foreach (var code in hashCodesForProperties)
                    hash = hash * FactorPrime + code;
                return hash;
            }
        }

        public static string GetPropertyNameFromExpression<T>(Expression<Func<T, object>> expr)
        {
            MemberExpression memberExpression = null;
            if (expr.Body.NodeType == ExpressionType.Convert)
            {
                memberExpression = ((UnaryExpression)expr.Body).Operand as MemberExpression;
            }
            else
            {
                memberExpression = expr.Body as MemberExpression;
            }

            return memberExpression?.Member?.Name;
        }

        /// <summary>
        /// Safely registers a list of values against a given key, by using the <see cref="ConcurrentDictionary{TKey, TValue}"/> implementation.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="key">The key of the values to be registered.</param>
        /// <param name="value">The value to be registered.</param>
        /// <param name="registry">The <see cref="ConcurrentDictionary{TKey, TValue}"/> instance which holds the registration.</param>
        public static void ConcurrentDictionarySafeRegister<TKey, TValue>(TKey key, TValue value, ConcurrentDictionary<TKey, List<TValue>> registry)
        {
            if (registry.TryGetValue(key, out List<TValue> registryItem))
            {
                if (registryItem != null)
                {
                    if (!registryItem.Contains(value))
                    {
                        registry[key].Add(value);
                    }
                }
                else
                {
                    registry[key] = new List<TValue> { value };
                }
            }
            else
            {
                registry.TryAdd(key, new List<TValue> { value });
            }
        }
    }
}
