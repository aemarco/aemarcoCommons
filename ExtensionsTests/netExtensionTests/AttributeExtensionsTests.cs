using Extensions.netExtensions;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Globalization;

namespace ExtensionsTests.netExtensionTests
{


    public class AttributeExtensionsTests
    {

        [TestCase(TestEnum.Test1, "111")]
        [TestCase(TestEnum.Test2, "222")]
        [TestCase(TestEnum.Test3, "333")]
        public void GetAttribute_Returns_CorrectlyA(TestEnum value, string info)
        {
            var result = value.GetAttribute<DummyAAttribute>();
            result.Info.Should().Be(info);
        }
        
        [TestCase(TestEnum.Test4, "444")]
        public void GetAttribute_Returns_CorrectlyB(TestEnum value, string info)
        {
            var result = value.GetAttribute<DummyBAttribute>();
            result.Info.Should().Be(info);
        }

        [Test]
        public void GetAttribute__Returns_Null()
        {
            const TestEnum value = TestEnum.Test4;
            var result = value.GetAttribute<DummyAAttribute>();
            result.Should().BeNull();
        }

        public enum TestEnum
        {
            [DummyA("111")]
            Test1,
            [DummyA("222")]
            Test2,
            [DummyA("333")]
            Test3,
            [DummyB("444")]
            Test4
        }


        private sealed class DummyAAttribute : Attribute
        {
            public DummyAAttribute(string info)
            {
                Info = info;
            }

            public string Info { get; }
        }

        private sealed class DummyBAttribute : Attribute
        {
            public DummyBAttribute(string info)
            {
                Info = info;
            }

            public string Info { get; }
        }

       
    }
}
