using aemarcoCommons.Extensions.CryptoExtensions;
using aemarcoCommons.Extensions.FileExtensions;
using FluentAssertions;
using NUnit.Framework;
using System.IO;

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

    [Test]
    public void EncryptDecryptFile_ShouldWork()
    {
        var bytes = Symetric.GetRandomBytes(8 * 1024 * 1024); //1mb

        var fileInfo = new FileInfo("testFile.fi");
        File.WriteAllBytes(fileInfo.FullName, bytes);

        fileInfo.EncryptFileInPlace("pass", KeySize.Highest_256);
        fileInfo.DecryptFileInPlace("pass");

        var fromFile = File.ReadAllBytes(fileInfo.FullName);
        fileInfo.Delete();

        bytes.Should().BeEquivalentTo(fromFile);
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

        var crypted = fileInfo.EncryptFile("pass", false, KeySize.Highest_256);
        var decrypted = crypted.DecryptFile("pass", true);


        var result = fileInfo.Base64HashFromFile() == decrypted.Base64HashFromFile();
        fileInfo.Delete();
        decrypted.Delete();
        result.Should().BeTrue();
    }

}