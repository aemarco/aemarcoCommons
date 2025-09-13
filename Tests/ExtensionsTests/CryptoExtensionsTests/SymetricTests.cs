using aemarcoCommons.Extensions.CryptoExtensions;
using aemarcoCommons.Extensions.FileExtensions;
using FluentAssertions;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;

namespace ExtensionsTests.CryptoExtensionsTests;

public class SymetricTests
{

    [TestCase("test")]
    [TestCase("77B80F6DE365762B070048D1F59ABB39")]
    [TestCase(@"C:\Users\someUser\someDir\someOtherDir\someTestDir\bin\Debug\net6.0\dataFolder\someFile-000-111.file")]
    public void EncryptDecrypt_ShouldWork(string text)
    {
        var encrypted = text.EncryptToBase64("password");
        var result = encrypted.DecryptFromBase64("password");

        result.Should().Be(text);
    }


    [TestCase("test")]
    [TestCase("77B80F6DE365762B070048D1F59ABB39")]
    [TestCase(@"C:\Users\someUser\someDir\someOtherDir\someTestDir\bin\Debug\net6.0\dataFolder\someFile-000-111.file")]
    public async Task EncryptDecrypt_ShouldWork_Async(string text)
    {
        var encrypted = await text.EncryptToBase64Async("password");
        var result = await encrypted.DecryptFromBase64Async("password");

        result.Should().Be(text);
    }

    [Test]
    public void EncryptDecryptFile_ShouldWork()
    {
        var bytes = Symetric.GetRandomBytes(8 * 1024 * 1024); //1mb

        var fileInfo = new FileInfo("testFile.fi");
        File.WriteAllBytes(fileInfo.FullName, bytes);

        fileInfo.EncryptFileInPlace("pass");
        fileInfo.DecryptFileInPlace("pass");

        var fromFile = File.ReadAllBytes(fileInfo.FullName);
        fileInfo.Delete();

        bytes.Should().Equal(fromFile);
    }

    [Test]
    public async Task EncryptDecryptFile_ShouldWork_Async()
    {
        var bytes = Symetric.GetRandomBytes(8 * 1024 * 1024); // 8 MB

        var fileInfo = new FileInfo("testFileAsync.fi");
        await File.WriteAllBytesAsync(fileInfo.FullName, bytes);

        var crypted = await fileInfo.EncryptFileAsync("pass", true);
        var decrypted = await crypted.DecryptFileAsync("pass", true);

        var fromFile = await File.ReadAllBytesAsync(decrypted.FullName);
        decrypted.Delete();
        bytes.Should().Equal(fromFile);

    }


    [Test]
    [Explicit]
    public void EncryptDecryptLargeFile_ShouldWork()
    {
        var fileInfo = new FileInfo("largeTestFile.fi");
        using (var ws = fileInfo.OpenWrite())
        {
            for (int i = 0; i < 3 * 1024; i++) //3Gb --> 1Mb per round
            {
                ws.Write(Symetric.GetRandomBytes(8 * 1024 * 1024));
            }
        }

        var crypted = fileInfo.EncryptFile("pass");
        var decrypted = crypted.DecryptFile("pass", true);


        var result = fileInfo.Base64HashFromFile() == decrypted.Base64HashFromFile();
        fileInfo.Delete();
        decrypted.Delete();
        result.Should().BeTrue();
    }
    [Test]
    [Explicit]
    public async Task EncryptDecryptLargeFile_ShouldWork_Async()
    {
        var fileInfo = new FileInfo("largeTestFileAsync.fi");
        await using (var ws = File.OpenWrite(fileInfo.FullName))
        {
            for (int i = 0; i < 3 * 1024; i++) // 3 GB --> 1MB per round
            {
                await ws.WriteAsync(Symetric.GetRandomBytes(8 * 1024 * 1024));
            }
        }

        var crypted = await fileInfo.EncryptFileAsync("pass");
        var decrypted = await crypted.DecryptFileAsync("pass", true);

        var result = fileInfo.Base64HashFromFile() == decrypted.Base64HashFromFile();

        fileInfo.Delete();
        decrypted.Delete();

        result.Should().BeTrue();
    }
}