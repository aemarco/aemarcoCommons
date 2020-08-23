using Extensions.TextExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace ExtensionsTests.TextExtensionsTests
{
    public class RegexExtensionsTests
    {
        [TestCase("david.jones@proseware.com", true)]
        [TestCase("d.j@server1.proseware.com", true)]
        [TestCase("jones@ms1.proseware.com", true)]
        [TestCase("j@proseware.com9", true)]
        [TestCase("js#internal@proseware.com", true)]
        [TestCase("j_9@[129.126.118.1]", true)]
        [TestCase("js@proseware.com9", true)]
        [TestCase("j.s@server1.proseware.com", true)]
        [TestCase("\"j\\\"s\\\"\"@proseware.com", true)]
        [TestCase("j.@server1.proseware.com", false)]
        [TestCase("j..s@proseware.com", false)]
        [TestCase("js*@proseware.com", false)]
        [TestCase("js@proseware..com", false)]
        public void Email_ValidatesCorrect(string email, bool expected)
        {
            var result = email.IsValidEmail();
            result.Should().Be(expected);
        }
    }
}
