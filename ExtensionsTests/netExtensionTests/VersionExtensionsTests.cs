using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Extensions.netExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace ExtensionsTests.netExtensionTests
{
    public class VersionExtensionsTests
    {
        
        
        [Test]
        public void DoesCompleteCircle()
        {
            var testVersion = Version.Parse("1.2.3.4");
            var fi = new FileInfo("version.json");
            if (fi.Exists) fi.Delete();


            testVersion.ToTextFile(fi.FullName);
            var result = fi.ToVersionFromTextFile(null);


            result.Should().NotBeNull();
            result.Should().Be(testVersion);


            fi.Delete();
        }

        [Test]
        public void ToVersionReturnsCorrectDefault()
        {
            var expected = Version.Parse("2.3.4.5");

            var result = new FileInfo("notThere.json")
                .ToVersionFromTextFile(expected);

            result.Should().Be(expected);
        }






    }
}
