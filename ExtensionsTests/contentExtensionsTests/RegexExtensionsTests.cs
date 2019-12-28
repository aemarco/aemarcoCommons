using Extensions.contentExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace ExtensionsTests.contentExtensionsTests
{
    class RegexExtensionsTests
    {
        [TestCase("david.jones@proseware.com", true, "valid")]
        [TestCase("d.j@server1.proseware.com", true, "valid")]
        [TestCase("jones@ms1.proseware.com", true, "valid")]
        [TestCase("j@proseware.com9", true, "valid")]
        [TestCase("js#internal@proseware.com", true, "valid")]
        [TestCase("j_9@[129.126.118.1]", true, "valid")]
        [TestCase("js@proseware.com9", true, "valid")]
        [TestCase("j.s@server1.proseware.com", true, "valid")]
        [TestCase("\"j\\\"s\\\"\"@proseware.com", true, "valid")]
        [TestCase("j.@server1.proseware.com", false, "invalid")]
        [TestCase("j..s@proseware.com", false, "invalid")]
        [TestCase("js*@proseware.com", false, "invalid")]
        [TestCase("js@proseware..com", false, "invalid")]
        public void Email_ValidatesCorrect(string email, bool expected, string reason)
        {
            var result = email.IsValidEmail();
            result.Should().Be(expected);
        }
    }
}
