using System.IO;
using Extensions.netExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace ExtensionsTests.netExtensionTests
{
    public class VersionExtensionsTests
    {
        
        
        //[Test]
        //public void DoesCompleteCircle()
        //{
        //    var testVersion = Version.Parse("1.2.3.4");
        //    var fi = new FileInfo("version.json");
        //    if (fi.Exists) fi.Delete();


        //    testVersion.ToTextFile(fi.FullName);
        //    var result = fi.ToVersionFromTextFile(null);


        //    result.Should().NotBeNull();
        //    result.Should().Be(testVersion);


        //    fi.Delete();
        //}

        [Test]
        public void ToVersionReturnsCorrectDefault()
        {
            
            var result = new FileInfo("notThere.json")
                .ToVersionFromTextFile("2.3.4.5");

            result.ToString().Should().Be("2.3.4.5");
        }






    }
}
