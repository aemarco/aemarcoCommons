using aemarcoCommons.Extensions.CryptoExtensions;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;
using System.Security.Cryptography;

namespace ExtensionsTests.CryptoExtensionsTests
{
    public class PasswordExtensionsTests
    {
        [TestCase("test")]
        [TestCase("77B80F6DE365762B070048D1F59ABB39")]
        [TestCase(@"C:\Users\someUser\someDir\someOtherDirrrrrr\someTestDir\bin\Debug\net6.0\dataFolder\someFile-000-111.file")]
        public void EncryptDecrypt_ShouldWork(string text)
        {
            var encrypted = text.Encrypt("password");
            var result = encrypted.Decrypt("password");

            result.Should().Be(text);
        }
    }
}
