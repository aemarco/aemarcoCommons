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
    [TestCase(@"C:\Users\someUser\someDir\someOtherDirrrrrr\someTestDir\bin\Debug\net6.0\dataFolder\someFile-000-111.file")]
    public void EncryptDecrypt_ShouldWork(string text)
    {
        var encrypted = text.EncryptToBase64("password");
        var result = encrypted.DecryptFromBase64("password");

        result.Should().Be(text);
    }

    [Test]
    public void EncryptDecryptFile_ShouldWork()
    {
        var bytes = Symetric.GetRandomBytes(1024 * 1024 * 8);

        var fileInfo = new FileInfo("testfile.fi");
        File.WriteAllBytes(fileInfo.FullName, bytes);

        fileInfo.EncryptFileInPlace("passs", KeySize.Highest_256);
        fileInfo.DecryptFileInPlace("passs");

        var fromFile = File.ReadAllBytes(fileInfo.FullName);
        fileInfo.Delete();

        bytes.Should().BeEquivalentTo(fromFile);
    }


    [Test]
    [Explicit]
    public void EncryptDecryptLargeFile_ShouldWork()
    {
        var fileInfo = new FileInfo("largetestfile.fi");
        using (var ws = fileInfo.OpenWrite())
        {
            for (int i = 0; i < 1024 * 3; i++) //3Gb --> 1Mb per round
            {
                ws.Write(Symetric.GetRandomBytes(1024 * 1024 * 8));
            }
        }

        var crypted = fileInfo.EncryptFile("passs", false, KeySize.Highest_256);
        var decrypted = crypted.DecryptFile("passs", true);


        var result = fileInfo.Base64HashFromFile() == decrypted.Base64HashFromFile();
        fileInfo.Delete();
        decrypted.Delete();
        result.Should().BeTrue();
    }

}