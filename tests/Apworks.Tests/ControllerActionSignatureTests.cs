using Apworks.Integration.AspNetCore.Hal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Apworks.Tests
{
    public class ControllerActionSignatureTests
    {
        [Fact]
        public void SimpleEqualsTest()
        {
            var cas1 = new ControllerActionSignature("Values", "Get", new[] { typeof(int) });
            var cas2 = new ControllerActionSignature("Values", "Get", new[] { typeof(int) });
            Assert.True(cas1.Equals(cas2));
        }

        [Fact]
        public void EqualsWithoutParameterTypesTest()
        {
            var cas1 = new ControllerActionSignature("Values", "Get");
            var cas2 = new ControllerActionSignature("Values", "Get");
            Assert.True(cas1.Equals(cas2));
        }

        [Fact]
        public void EqualsWithIgnoreCaseTest()
        {
            var cas1 = new ControllerActionSignature("values", "Get");
            var cas2 = new ControllerActionSignature("Values", "get");
            Assert.True(cas1.Equals(cas2));
        }

        [Fact]
        public void EqualsNullTest()
        {
            var cas1 = new ControllerActionSignature("values", "Get");
            ControllerActionSignature cas2 = null;
            Assert.False(cas1.Equals(cas2));
        }

        [Fact]
        public void EqualsOperatorOverrideTest()
        {
            var cas1 = new ControllerActionSignature("Values", "Get", new[] { typeof(int) });
            var cas2 = new ControllerActionSignature("Values", "Get", new[] { typeof(int) });
            Assert.True(cas1 == cas2);
        }

        [Fact]
        public void EqualsOperatorOverrideWithoutParameterTypesTest()
        {
            var cas1 = new ControllerActionSignature("Values", "Get");
            var cas2 = new ControllerActionSignature("Values", "Get");
            Assert.True(cas1 == cas2);
        }

        [Fact]
        public void EqualsOperatorOverrideWithIgnoreCaseTest()
        {
            var cas1 = new ControllerActionSignature("values", "Get");
            var cas2 = new ControllerActionSignature("Values", "get");
            Assert.True(cas1 == cas2);
        }

        [Fact]
        public void EqualsOperatorOverrideNullTest()
        {
            var cas1 = new ControllerActionSignature("values", "Get");
            ControllerActionSignature cas2 = null;
            Assert.False(cas1 == cas2);
        }

        [Fact]
        public void SignatureWithDifferentParamTest()
        {
            var cas1 = new ControllerActionSignature("Values", "Get", new[] { typeof(int) });
            var cas2 = new ControllerActionSignature("Values", "Get", new[] { typeof(string) });

            Assert.False(cas1 == cas2);
        }

        [Fact]
        public void ControllerInsensitiveTest()
        {
            var cas1 = new ControllerActionSignature("Get", new[] { typeof(int) });
            var cas2 = new ControllerActionSignature("Values", "get", new[] { typeof(int) });
            Assert.True(cas1 == cas2);
        }

        [Fact]
        public void ControllerInsensitiveWithDifferentParamTest()
        {
            var cas1 = new ControllerActionSignature("Get", new[] { typeof(int) });
            var cas2 = new ControllerActionSignature("Values", "get", new[] { typeof(string) });
            Assert.False(cas1 == cas2);
        }

        [Fact]
        public void ImplicitConvertTest()
        {
            ControllerActionSignature cas = "values.get";
            Assert.True(cas.ControllerName == "values");
            Assert.True(cas.ActionName == "get");
        }

        [Fact]
        public void ImplicitConvertWithAnyControllerNameTest()
        {
            ControllerActionSignature cas = "*.get";
            Assert.True(cas.ControllerName == "*");
            Assert.True(cas.ActionName == "get");
        }

        [Fact]
        public void ImplicitConvertWithOneParameterTest()
        {
            ControllerActionSignature cas = "values.get(int)";
            Assert.True(cas.ControllerName == "values");
            Assert.True(cas.ActionName == "get");
            Assert.Equal(1, cas.ParameterTypes.Count());
            Assert.True(cas.ParameterTypes.First().Equals(typeof(int)));
        }

        [Fact]
        public void ImplicitConvertWithMultipleParametersTest()
        {
            ControllerActionSignature cas = "values.get(int ,  string   ,    double?)";
            Assert.True(cas.ControllerName == "values");
            Assert.True(cas.ActionName == "get");
            Assert.Equal(3, cas.ParameterTypes.Count());
            Assert.True(cas.ParameterTypes.First().Equals(typeof(int)));
            Assert.True(cas.ParameterTypes.Skip(1).Take(1).First().Equals(typeof(string)));
            Assert.True(cas.ParameterTypes.Last().Equals(typeof(double?)));
        }

        [Fact]
        public void ImplicitConvertWithWildcardInParametersButParameterCountMismatchTest()
        {
            ControllerActionSignature cas = "values.get(int,*,*)";
            ControllerActionSignature cas1 = "values.get(int)";
            Assert.False(cas == cas1);
        }

        [Fact]
        public void ImplicitConvertWithWildcardInParametersAndParameterCountMatches1Test()
        {
            ControllerActionSignature cas = "values.get(int,*,*)";
            ControllerActionSignature cas1 = "values.get(int, string, double?)";
            Assert.True(cas == cas1);
        }

        [Fact]
        public void ImplicitConvertWithWildcardInParametersAndParameterCountMatches2Test()
        {
            ControllerActionSignature cas = "values.get(int,*,*)";
            ControllerActionSignature cas1 = "values.get(int16, string, double?)";
            Assert.False(cas == cas1);
        }
    }
}

