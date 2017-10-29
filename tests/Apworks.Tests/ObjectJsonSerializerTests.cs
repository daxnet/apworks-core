using Apworks.Serialization.Json;
using Apworks.Tests.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Apworks.Tests
{
    public class ObjectJsonSerializerTests
    {
        [Fact]
        public void StronglyTypedSerializeDeserializeTest()
        {
            var customer = Customer.CreateOne();
            var ser = new ObjectJsonSerializer();
            var bin = ser.Serialize<Customer>(customer);
            var customer2 = ser.Deserialize<Customer>(bin);
            Assert.Equal(customer.Id, customer2.Id);
            Assert.Equal(customer.Name, customer2.Name);
            Assert.Equal(customer.Email, customer2.Email);
            Assert.Equal(customer.DateRegistered, customer2.DateRegistered);
        }

        [Fact]
        public void NoTypeSerializeDeserializeTest()
        {
            Customer customer = Customer.CreateOne();
            var ser = new ObjectJsonSerializer();
            var bin = ser.Serialize(customer);
            Customer customer2 = ser.Deserialize<Customer>(bin);

            Assert.Equal(customer.Id, customer2.Id);
            Assert.Equal(customer.Name, customer2.Name);
            Assert.Equal(customer.Email, customer2.Email);
            Assert.Equal(customer.DateRegistered, customer2.DateRegistered);
        }

        [Fact]
        public void TypedSerializeDeserializeTest()
        {
            var customer = Customer.CreateOne();
            var ser = new ObjectJsonSerializer();
            var bin = ser.Serialize(typeof(Customer), customer);
            var customer2 = (Customer)ser.Deserialize(bin, typeof(Customer));

            Assert.Equal(customer.Id, customer2.Id);
            Assert.Equal(customer.Name, customer2.Name);
            Assert.Equal(customer.Email, customer2.Email);
            Assert.Equal(customer.DateRegistered, customer2.DateRegistered);
        }
    }
}
