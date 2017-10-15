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
            object customer = Customer.CreateOne();
            var ser = new ObjectJsonSerializer();
            var bin = ser.Serialize(customer);
            dynamic customer2 = ser.Deserialize(bin);

            Assert.Equal(((Customer)customer).Id, customer2.Id);
            Assert.Equal(((Customer)customer).Name, customer2.Name);
            Assert.Equal(((Customer)customer).Email, customer2.Email);
            Assert.Equal(((Customer)customer).DateRegistered, customer2.DateRegistered);
        }

        [Fact]
        public void TypedSerializeDeserializeTest()
        {
            object customer = Customer.CreateOne();
            var ser = new ObjectJsonSerializer();
            var bin = ser.Serialize(typeof(Customer), customer);
            var customer2 = (Customer)ser.Deserialize(typeof(Customer), bin);

            Assert.Equal(((Customer)customer).Id, customer2.Id);
            Assert.Equal(((Customer)customer).Name, customer2.Name);
            Assert.Equal(((Customer)customer).Email, customer2.Email);
            Assert.Equal(((Customer)customer).DateRegistered, customer2.DateRegistered);
        }
    }
}
