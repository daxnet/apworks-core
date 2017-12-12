using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Apworks.Tests
{
    public class TestTest
    {
        public TestTest()
        {
        }

        // [Fact]
        public void PassingTest()
        {
            Assert.Equal(4, 2 + 2);
        }
    }
}
