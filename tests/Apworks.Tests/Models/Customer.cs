using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Tests.Models
{
    public class Customer : IAggregateRoot<int>
    {
        private static readonly Random rnd = new Random(DateTime.UtcNow.Millisecond);

        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj == null)
            {
                return false;
            }

            var other = obj as Customer;
            if (other == null)
            {
                return false;
            }

            return this.Id.Equals(other.Id) &&
                this.Name.Equals(other.Name) &&
                this.Email.Equals(other.Email);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ this.Name.GetHashCode() ^ this.Email.GetHashCode();
        }

        public static Customer CreateOne() => CreateOne(rnd.Next(), $"Customer Name {rnd.Next()}", $"CustomerEmail{rnd.Next()}@domain.com");

        public static Customer CreateOne(int id, string name, string email)
        {
            return new Customer
            {
                Id = id,
                Name = name,
                Email = email
            };
        }

        public static IList<Customer> CreateMany(int num = 10)
        {
            var result = new List<Customer>();
            for (var i = 0; i < num; i++)
            {
                result.Add(CreateOne());
            }

            return result;
        }
    }
}
