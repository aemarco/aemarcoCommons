using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aemarcoCommons.Extensions.CollectionExtensions;
using aemarcoCommons.Extensions.CryptoExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace ExtensionsTests.CollectionTests
{
    public class ConditionalsTests
    {
        [TestCase(null, false)]
        [TestCase(new string[] { }, false)]
        [TestCase(new[] { "bob" }, true)]
        public void NotNullOrEmpty_Delivers(string[] collection, bool expected)
        {

            var result = collection.NotNullOrEmpty();
            result.Should().Be(expected);
        }

        [TestCase(null, true)]
        [TestCase(new string[] { }, true)]
        [TestCase(new[] { "bob" }, false)]
        public void NullOrEmpty_Delivers(string[] collection, bool expected)
        {

            var result = collection.NullOrEmpty();
            result.Should().Be(expected);
        }


    }
}
