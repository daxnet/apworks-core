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

using Apworks.Events;
using Apworks.Snapshots;
using Apworks.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Apworks
{
    /// <summary>
    /// Represents the base class for all aggregate roots, with the domain event capability for
    /// both standard event-driven and CQRS-based applications.
    /// </summary>
    /// <typeparam name="TKey">The type of the aggregate root key.</typeparam>
    public abstract class AggregateRootWithEventSourcing<TKey> : IAggregateRootWithEventSourcing<TKey>, ISnapshotOriginator
        where TKey : IEquatable<TKey>
    {
        private static readonly object lockObj = new object();
        private readonly Queue<IDomainEvent> uncommittedEvents = new Queue<IDomainEvent>();
        private readonly Lazy<ConcurrentDictionary<string, IEnumerable<MethodInfo>>> eventHandlerRegistrations = new Lazy<ConcurrentDictionary<string, IEnumerable<MethodInfo>>>();
        private long persistedVersion;

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public TKey Id { get; set; }

        /// <summary>
        /// Gets a list of <see cref="IDomainEvent"/> 
        /// </summary>
        public IEnumerable<IDomainEvent> UncommittedEvents => uncommittedEvents;

        private void HandleEvent<TEvent>(TEvent @event)
            where TEvent : class, IDomainEvent
        {
            if (!eventHandlerRegistrations.IsValueCreated)
            {
                var registrations = from method in this.GetType()
                                    .GetTypeInfo()
                                    .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                    let parameters = method.GetParameters()
                                    let handlesAttribute = method.GetCustomAttributes<HandlesAttribute>().ToList().FirstOrDefault()
                                    where handlesAttribute != null &&
                                        method.ReturnType == typeof(void) &&
                                        parameters.Length == 1 &&
                                        typeof(IDomainEvent).GetTypeInfo().IsAssignableFrom(parameters[0].ParameterType) &&
                                        handlesAttribute.EventType == parameters[0].ParameterType
                                    group method by handlesAttribute.EventType.AssemblyQualifiedName into methodGroup
                                    select new { EventTypeName = methodGroup.Key, Methods = methodGroup.ToList() };

                foreach(var registration in registrations)
                {
                    eventHandlerRegistrations.Value[registration.EventTypeName] = registration.Methods;
                }
            }

            var registrationKey = @event.GetType().AssemblyQualifiedName;
            var succeeded = this.eventHandlerRegistrations.Value.TryGetValue(registrationKey, out IEnumerable<MethodInfo> handlers);
            if (succeeded && handlers.Count() > 0)
            {
                foreach(var handler in handlers)
                {
                    handler.Invoke(this, new[] { @event });
                }
            }
            else
            {
                this.CopyFromEvent(@event);
            }
        }

        /// <summary>
        /// Copies the value of each readable property in <paramref name="event"/> object
        /// and tries to set the value to the corresponding writable property in the current
        /// aggregate instance.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="event">The event object from which the value is retrieved and being mapped
        /// to the current aggregate instance.</param>
        private void CopyFromEvent<TEvent>(TEvent @event)
            where TEvent : class, IDomainEvent
        {
            var eventProperties = from p in @event.GetType()
                                      .GetTypeInfo()
                                      .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                  where p.PropertyType.IsSimpleType() && 
                                        p.CanRead && 
                                        !p.Name.Equals("Id", StringComparison.CurrentCultureIgnoreCase) &&
                                        !p.Name.Equals("Timestamp", StringComparison.CurrentCultureIgnoreCase)
                                  select p;

            foreach(var eventProperty in eventProperties)
            {
                var curProperty = (from p in this.GetType().GetTypeInfo()
                                      .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                   where p.PropertyType == eventProperty.PropertyType &&
                                         p.Name.Equals(eventProperty.Name, StringComparison.CurrentCultureIgnoreCase) &&
                                         p.CanWrite
                                   select p).FirstOrDefault();

                if (curProperty != null)
                {
                    curProperty.SetValue(this, eventProperty.GetValue(@event));
                }
            }
        }

        /// <summary>
        /// Raises the specified domain event, which will put the specified event into
        /// the uncommitted events queue.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to be raised.</typeparam>
        /// <param name="event">The object which contains the event data.</param>
        protected void Raise<TEvent>(TEvent @event)
            where TEvent : class, IDomainEvent
        {
            @event.Sequence = this.Version + 1;
            this.uncommittedEvents.Enqueue(@event);
        }

        /// <summary>
        /// Applies the specified domain event. This will cause the event to be handled
        /// in the current aggregate root and then be put into the uncommitted events
        /// queue.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to be raised.</typeparam>
        /// <param name="event">The object which contains the event data.</param>
        /// <exception cref="ArgumentNullException">event</exception>
        protected void Apply<TEvent>(TEvent @event)
            where TEvent : class, IDomainEvent
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            @event.Id = Guid.NewGuid();
            @event.Timestamp = DateTime.UtcNow;
            @event.AttachTo(this);

            this.HandleEvent(@event);
            this.Raise(@event);
        }

        /// <summary>
        /// Applies the specified domain event. This will cause the event to be handled
        /// in the current aggregate root and then be put into the uncommitted events
        /// queue.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to be raised.</typeparam>
        protected void Apply<TEvent>()
            where TEvent : class, IDomainEvent, new()
        {
            this.Apply(new TEvent());
        }

        /// <summary>
        /// Replays the domain events one by one to restore the aggregate state.
        /// </summary>
        /// <param name="domainEvents">The domain events to be replayed on the current aggregate.</param>
        public void Replay(IEnumerable<IDomainEvent> domainEvents)
        {
            ((IPurgeable)this).Purge();
            domainEvents.OrderBy(e => e.Timestamp)
                .ToList()
                .ForEach(de =>
                {
                    this.HandleEvent(de);
                    Interlocked.Increment(ref this.persistedVersion);
                });
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var other = obj as AggregateRootWithEventSourcing<TKey>;
            if (other == null)
            {
                return false;
            }

            return this.Id.Equals(other.Id);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Utils.GetHashCode(this.Id.GetHashCode());
        }

        public long Version { get => this.persistedVersion + this.uncommittedEvents.Count; }

        /// <summary>
        /// Removes all the uncommitted events, if any, from the list.
        /// </summary>
        void IPurgeable.Purge()
        {
            if (this.uncommittedEvents.Count > 0)
            {
                lock(lockObj)
                {
                    this.persistedVersion += this.uncommittedEvents.Count;
                }

                this.uncommittedEvents.Clear();
            }
        }

        public virtual ISnapshot TakeSnapshot()
        {
            var snapshot = new Snapshot();
            snapshot.Version = this.Version;
            snapshot.Timestamp = DateTime.UtcNow;
            var properties = from p in this.GetType().GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             where p.PropertyType.IsSimpleType() &&
                                p.CanRead && p.CanWrite
                             select p;
            foreach(var property in properties)
            {
                snapshot.AddState(property.Name, property.GetValue(this));
            }

            return snapshot;
        }

        public virtual void RestoreSnapshot(ISnapshot snapshot)
        {
            this.persistedVersion = snapshot.Version;
            var thisType = this.GetType().GetTypeInfo();
            foreach (var kvp in snapshot.States)
            {
                var prop = thisType.GetProperty(kvp.Key, BindingFlags.Public | BindingFlags.Instance);
                prop?.SetValue(this, kvp.Value);
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(AggregateRootWithEventSourcing<TKey> a, AggregateRootWithEventSourcing<TKey> b)
        {
            if ((object)a == null)
            {
                return (object)b == null;
            }

            return a.Equals(b);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(AggregateRootWithEventSourcing<TKey> a, AggregateRootWithEventSourcing<TKey> b) => !(a == b);
    }
}
