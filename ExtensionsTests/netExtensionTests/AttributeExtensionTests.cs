using Extensions.netExtensions;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace ExtensionsTests.netExtensionTests
{


    public class AttributeExtensionTests
    {

        [Test]
        public void ToAttribute_Returns_EnumAttA()
        {
            var value = TestEnum.Test1;
            var result = value.GetAttribute<DummyAAttribute>();


            result.Info.Should().Be("111");
        }


        [Test]
        public void ToAttribute__Returns_EnumAttNull()
        {
            var value = TestEnum.Test4;
            var result = value.GetAttribute<DummyAAttribute>();

            result.Should().BeNull();
        }

        [Test]
        public void ToAttribute_Returns_EnumAttB()
        {
            var value = TestEnum.Test4;
            var result = value.GetAttribute<DummyBAttribute>();


            result.Info.Should().Be("444");
        }


    }



    internal sealed class DummyAAttribute : Attribute
    {
        public DummyAAttribute(string info)
        {
            Info = info;
        }
        public string Info { get; }
    }

    internal sealed class DummyBAttribute : Attribute
    {
        public DummyBAttribute(string info)
        {
            Info = info;
        }
        public string Info { get; }
    }

    internal enum TestEnum
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

}
