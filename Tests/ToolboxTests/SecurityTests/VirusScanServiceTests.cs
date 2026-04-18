using aemarcoCommons.Extensions.FileExtensions;
using aemarcoCommons.Toolbox.SecurityTools;

namespace ToolboxTests.SecurityTests;

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

        result.Success.ShouldBeTrue();
        result.IsThread.ShouldBe(true);
        result.Exception.ShouldBeNull();
    }

    [Test]
    public void ScanFile_DeclaresSave()
    {
        var result = new VirusScanService().ScanFile("save");

        result.Success.ShouldBeTrue();
        result.IsThread.ShouldBe(false);
        result.Exception.ShouldBeNull();
    }

    [Test]
    public void ScanFile_ThrowsFileNotFound_ForScanner()
    {
        var result = new VirusScanService().ScanFile("save", "123.exe");

        result.Success.ShouldBeFalse();
        result.IsThread.ShouldBeNull();
        result.Exception.ShouldBeOfType<FileNotFoundException>();
    }

    [Test]
    public void ScanFile_ThrowsFileNotFound_ForCandidate()
    {
        var result = new VirusScanService().ScanFile("notThere");

        result.Success.ShouldBeFalse();
        result.IsThread.ShouldBeNull();
        result.Exception.ShouldBeOfType<FileNotFoundException>();
    }

}