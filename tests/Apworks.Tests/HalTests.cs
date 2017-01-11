using Apworks.Integration.AspNetCore.Hal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Apworks.Tests
{
    public class HalTests
    {
        [Fact]
        public void SerializeLinkItemTest()
        {
            var linkItem = new LinkItem
            {
                Href = "/orders"
            };

            linkItem.AddProperty("title", "kate");
            linkItem.AddProperty("age", 10);

            var json = linkItem.ToJson();
        }

        [Fact]
        public void SerializeLinkItemCollectionTest()
        {
            var linkItem = new LinkItem
            {
                Href = "/orders"
            };

            linkItem.AddProperty("title", "kate");
            linkItem.AddProperty("age", 10);

            var linkItemCollection = new LinkItemCollection();
            linkItemCollection.Add(linkItem);

            var json = linkItemCollection.ToJson();
        }
    }
}
