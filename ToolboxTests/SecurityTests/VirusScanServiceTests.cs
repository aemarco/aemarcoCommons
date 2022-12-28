using aemarcoCommons.Extensions.FileExtensions;
using aemarcoCommons.Toolbox.SecurityTools;
using FluentAssertions;
using NUnit.Framework;
using System.IO;

namespace ToolboxTests.SecurityTests
{
    public class VirusScanServiceTests
    {

        [OneTimeSetUp]
        public void SetupStuff()
        {
            var eicar = @"X5O!P%@AP[4\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*"u8.ToArray();
            File.WriteAllBytes("eicar", eicar);

            var save = @"123"u8.ToArray();
            File.WriteAllBytes("save", save);
        }

        [OneTimeTearDown]
        public void DeleteStuff()
        {
            new FileInfo("eicar").TryDelete();
            new FileInfo("save").TryDelete();
        }


        [Test]
        public void ScanFile_Detects()
        {
            var result = new VirusScanService().ScanFile("eicar");

            result.Success.Should().BeTrue();
            result.IsThread.Should().BeTrue();
            result.Exception.Should().BeNull();
        }

        [Test]
        public void ScanFile_DeclaresSave()
        {
            var result = new VirusScanService().ScanFile("save");

            result.Success.Should().BeTrue();
            result.IsThread.Should().BeFalse();
            result.Exception.Should().BeNull();
        }

        [Test]
        public void ScanFile_ThrowsFileNotFound_ForScanner()
        {
            var result = new VirusScanService().ScanFile("save", "123.exe");

            result.Success.Should().BeFalse();
            result.IsThread.Should().BeNull();
            result.Exception.Should().BeOfType(typeof(FileNotFoundException));
        }

        [Test]
        public void ScanFile_ThrowsFileNotFound_ForCandidate()
        {
            var result = new VirusScanService().ScanFile("notThere");

            result.Success.Should().BeFalse();
            result.IsThread.Should().BeNull();
            result.Exception.Should().BeOfType(typeof(FileNotFoundException));
        }

    }
}
