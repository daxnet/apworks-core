using Apworks.Events;
using Apworks.EventStore.AdoNet;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace Apworks.Tests
{
    public class AdoNetEventStoreConfigurationTests
    {
        [Fact]
        public void AssignConnectionStringTest()
        {
            var config = new AdoNetEventStoreConfiguration("conn");
            Assert.Equal("conn", config.ConnectionString);
        }

        [Fact]
        public void DefaultTableNameTest()
        {
            var config = new AdoNetEventStoreConfiguration("conn");
            Assert.Equal("EVENTS", config.TableName);
        }

        [Fact]
        public void DefaultFieldNameTest1()
        {
            var config = new AdoNetEventStoreConfiguration("conn");
            Assert.Equal("EVENTCLRTYPE", config.GetFieldName(x => x.EventClrType));
        }

        [Fact]
        public void DefaultFieldNameTest2()
        {
            var config = new AdoNetEventStoreConfiguration("conn");
            Assert.Equal("EVENTCLRTYPE", config.GetFieldName("EventClrType"));
        }

        [Fact]
        public void DefaultFieldNameTest3()
        {
            var config = new AdoNetEventStoreConfiguration("conn");
            Assert.Null(config.GetFieldName("EventClrType1"));
        }

        [Fact]
        public void OverriddenTableNameTest()
        {
            var config = new AdoNetEventStoreConfiguration("conn", "table");
            Assert.Equal("table", config.TableName);
        }

        [Fact]
        public void OverriddenFieldNameTest1()
        {
            var config = new AdoNetEventStoreConfiguration("conn", new KeyValuePair<Expression<Func<EventDescriptor, object>>, string>(x=>x.EventClrType, "CLRTYPE"));
            Assert.Equal("CLRTYPE", config.GetFieldName(x => x.EventClrType));
        }

        [Fact]
        public void OverriddenFieldNameTest2()
        {
            var config = new AdoNetEventStoreConfiguration("conn", new KeyValuePair<Expression<Func<EventDescriptor, object>>, string>(x => x.EventClrType, "CLRTYPE"));
            Assert.Equal("CLRTYPE", config.GetFieldName(x => x.EventClrType));
            Assert.Equal("EVENTPAYLOAD", config.GetFieldName(x => x.EventPayload));
        }
    }
}
