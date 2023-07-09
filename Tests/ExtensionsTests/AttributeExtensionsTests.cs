using aemarcoCommons.Extensions;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;

namespace ExtensionsTests;
public class AttributeExtensionsTests
{
    [TestCase(typeof(TestClassA), null)]
    [TestCase(typeof(TestClassB), "abc")]
    public void GetAttribute_Returns_CorrectlyThroughType(Type type, string expected)
    {
        var attr = type.GetAttribute<DummyAAttribute>();
        var result = attr?.Info;

        result.Should().Be(expected);

    }

    [TestCase(typeof(TestClassA), null)]
    [TestCase(typeof(TestClassB), "xxx")]
    public void GetAttribute_Returns_CorrectlyThroughMember(Type type, string expected)
    {
        var attr = type.GetAttribute<DummyBAttribute>(nameof(TestClassB.Info));
        var result = attr?.Info;

        result.Should().Be(expected);

    }


    [TestCase(typeof(TestClassA), null)]
    [TestCase(typeof(TestClassB), "abc")]
    public void GetAttribute_Returns_CorrectlyThroughObject(Type type, string expected)
    {
        var obj = Activator.CreateInstance(type);
        var attr = obj.GetAttribute<DummyAAttribute>();
        var result = attr?.Info;
        result.Should().Be(expected);
    }

    [TestCase(typeof(TestClassA), null)]
    [TestCase(typeof(TestClassB), "xxx")]
    public void GetAttribute_Returns_CorrectlyThroughObjectMember(Type type, string expected)
    {
        var obj = Activator.CreateInstance(type);
        var attr = obj.GetAttribute<DummyBAttribute>(nameof(TestClassB.Info));
        var result = attr?.Info;

        result.Should().Be(expected);

    }


    [TestCase(typeof(TestClassA), false)]
    [TestCase(typeof(TestClassB), true)]
    public void HasAttribute_Returns_CorrectlyThroughObject(Type type, bool expected)
    {
        var obj = Activator.CreateInstance(type);
        var result = obj.HasAttribute<DummyAAttribute>();

        result.Should().Be(expected);

    }

    [TestCase(typeof(TestClassA), false)]
    [TestCase(typeof(TestClassB), true)]
    public void HasAttribute_Returns_CorrectlyThroughObjectMember(Type type, bool expected)
    {
        var obj = Activator.CreateInstance(type);
        var result = obj.HasAttribute<DummyBAttribute>(nameof(TestClassB.Info));

        result.Should().Be(expected);

    }


    [TestCase(typeof(TestClassA), 0)]
    [TestCase(typeof(TestClassB), 1)]
    public void GetAttributes_Returns_CorrectlyThroughType(Type type, int expected)
    {
        var list = type.GetAttributes<DummyAAttribute>();
        var result = list.Count();


        result.Should().Be(expected);
    }

    [TestCase(typeof(TestClassA), 0)]
    [TestCase(typeof(TestClassB), 1)]
    public void GetAttributes_Returns_CorrectlyThroughObject(Type type, int expected)
    {
        var obj = Activator.CreateInstance(type);
        var list = obj.GetAttributes<DummyAAttribute>();
        var result = list.Count();


        result.Should().Be(expected);
    }


    [TestCase(typeof(TestClassA), 0)]
    [TestCase(typeof(TestClassB), 1)]
    public void GetAttributes_Returns_CorrectlyThroughTypeMember(Type type, int expected)
    {
        var list = type.GetAttributes<DummyBAttribute>(nameof(TestClassB.Info));
        var result = list.Count();


        result.Should().Be(expected);
    }

    [TestCase(typeof(TestClassA), 0)]
    [TestCase(typeof(TestClassB), 1)]
    public void GetAttributes_Returns_CorrectlyThroughObjectMember(Type type, int expected)
    {
        var obj = Activator.CreateInstance(type);
        var list = obj.GetAttributes<DummyBAttribute>(nameof(TestClassB.Info));
        var result = list.Count();


        result.Should().Be(expected);
    }


    //with enums
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

    #region testclasses

    private sealed class TestClassA
    {

    }

    [DummyA("abc")]
    private sealed class TestClassB
    {
        [DummyB("xxx")]
        public string? Info { get; set; }
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

    #endregion

}
