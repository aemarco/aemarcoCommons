using aemarcoCommons.Toolbox.SecurityTools;
using FluentAssertions;
using NUnit.Framework;
using System.IO;
using System.Text;

namespace ToolboxTests.SecurityTests
{
    public class VirusScanServiceTests
    {
        [Test]
        public void ScanFile_Detects()
        {
            var eicar = Encoding.ASCII.GetBytes(@"X5O!P%@AP[4\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*");
            File.WriteAllBytes("eicar", eicar);


            var result = new VirusScanService().ScanFile("eicar");

            result.Success.Should().BeTrue();
            result.IsThread.Should().BeTrue();
        }

        [Test]
        public void ScanFile_DeclaresSave()
        {
            var save = Encoding.ASCII.GetBytes(@"123");
            File.WriteAllBytes("save", save);


            var result = new VirusScanService().ScanFile("save");

            result.Success.Should().BeTrue();
            result.IsThread.Should().BeFalse();
        }

        [Test]
        public void ScanFile_ThrowsFileNotFound_ForCandidate()
        {
            var result = new VirusScanService().ScanFile("save");

            result.Success.Should().BeFalse();
            result.IsThread.Should().BeNull();
            result.Exception.Should().BeOfType(typeof(FileNotFoundException));
        }

        [Test]
        public void ScanFile_ThrowsFileNotFound_ForScanner()
        {
            var save = Encoding.ASCII.GetBytes(@"123");
            File.WriteAllBytes("save", save);

            var result = new VirusScanService().ScanFile("save", "123.exe");

            result.Success.Should().BeFalse();
            result.IsThread.Should().BeNull();
            result.Exception.Should().BeOfType(typeof(FileNotFoundException));
        }





        [TearDown]
        public void DeleteStuff()
        {
            if (File.Exists("eicar")) File.Delete("eicar");
            if (File.Exists("save")) File.Delete("save");
        }
    }
}
