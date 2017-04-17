using Apworks.Events;
using Apworks.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Apworks
{
    /// <summary>
    /// Represents the base class for all aggregate roots, with the domain event capability for
    /// both standard event-driven and CQRS-based applications.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class AggregateRoot<TKey> : IAggregateRoot<TKey>, IPurgeable
        where TKey : IEquatable<TKey>
    {
        private readonly Queue<IDomainEvent> uncommittedEvents = new Queue<IDomainEvent>();
        private readonly Lazy<ConcurrentDictionary<string, IEnumerable<MethodInfo>>> eventHandlerRegistrations = new Lazy<ConcurrentDictionary<string, IEnumerable<MethodInfo>>>();

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
            where TEvent : class, IDomainEvent => this.uncommittedEvents.Enqueue(@event);

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
        /// <param name="domainEvents"></param>
        public void Replay(IEnumerable<IDomainEvent> domainEvents)
        {
            ((IPurgeable)this).Purge();
            domainEvents.OrderBy(e => e.Timestamp)
                .ToList()
                .ForEach(de => this.HandleEvent(de));
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

            var other = obj as AggregateRoot<TKey>;
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

        /// <summary>
        /// Removes all the uncommitted events, if any, from the list.
        /// </summary>
        void IPurgeable.Purge()
        {
            if (this.uncommittedEvents.Count > 0)
            {
                this.uncommittedEvents.Clear();
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
        public static bool operator ==(AggregateRoot<TKey> a, AggregateRoot<TKey> b)
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
        public static bool operator !=(AggregateRoot<TKey> a, AggregateRoot<TKey> b) => !(a == b);
    }
}
