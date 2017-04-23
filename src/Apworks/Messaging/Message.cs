using Apworks.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    /// <summary>
    /// Represents that the implemented classes are messages.
    /// </summary>
    /// <seealso cref="Apworks.Messaging.IMessage" />
    public abstract class Message : IMessage
    {
        private readonly MessageMetadata metadata = new MessageMetadata();

        protected Message()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets the identifier of the message.
        /// </summary>
        /// <value>
        /// The identifier of the message.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the timestamp which describes when the current message occurs.
        /// </summary>
        /// <value>
        /// The timestamp which describes when the current message occurs.
        /// </value>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets an instance of <see cref="T:Apworks.Messaging.MessageMetadata" /> class which contains the
        /// metadata information of current message.
        /// </summary>
        public MessageMetadata Metadata => metadata;

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => this.Id.ToString();

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

            var message = obj as Message;
            if (message == null)
            {
                return false;
            }

            return this.Id.Equals(message.Id);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => Utils.GetHashCode(this.Id.GetHashCode(), this.Timestamp.GetHashCode());
        
    }
}
