using Apworks.Integration.AspNetCore.Hal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Apworks.Tests
{
    public class HalTests
    {
        [Fact]
        public void SerializeLinkItemNoFormatTest()
        {
            var origin = File.ReadAllText("Data\\SerializeLinkItemNoFormatTest.txt");

            var linkItem = new LinkItem
            {
                Href = "/orders"
            };

            linkItem.AddProperty("title", "kate");
            linkItem.AddProperty("age", 10);

            var json = linkItem.ToJson(HalGenerationOption.NoFormat);

            Assert.Equal(origin, json);
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

            var json = linkItemCollection.ToJson(HalGenerationOption.Default);
        }
    }
}
