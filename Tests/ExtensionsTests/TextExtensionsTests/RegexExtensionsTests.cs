using aemarcoCommons.Extensions.TextExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace ExtensionsTests.TextExtensionsTests;

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

    //[TestCase("user@example.com", true)]
    //[TestCase("user.name@example.com", true)]
    //[TestCase("user.name+tag@example.com", true)]
    //[TestCase("user@example.co.uk", true)]
    //[TestCase("user@1.2.3.4", true)]
    //[TestCase("user@[IPv6:1:2:3::4]", true)]
    //[TestCase("invalid", false)]
    //[TestCase("invalid@", false)]
    //[TestCase("invalid@.", false)]
    public void Email_ValidatesCorrect(string email, bool expected)
    {
        var result = email.IsValidEmail();
        result.Should().Be(expected);
    }








    [TestCase("2023-12-31", 2023, 12, 31)]
    [TestCase("31-12-23", 2023, 12, 31)]
    [TestCase("12-31-23", 2023, 12, 31)]
    [TestCase("20231231", 2023, 12, 31)]
    [TestCase("31/12/23", 2023, 12, 31)]
    [TestCase("2023_12_31", 2023, 12, 31)]
    [TestCase("2023.12.31", 2023, 12, 31)]
    [TestCase("2023/12/31", 2023, 12, 31)]
    [TestCase("2023", 2023, 1, 1)]
    [TestCase("1080", null, null, null)] // ignored
    [TestCase("random text 2024-05-06 extra", 2024, 5, 6)]
    [TestCase("only numbers 720", null, null, null)]
    [TestCase("", null, null, null)]
    [TestCase(null, null, null, null)]
    public void ToDateTimeOffset_VariousFormats_ReturnsExpected(
        string input,
        int? expectedYear, int? expectedMonth, int? expectedDay)
    {
        var result = input.ToDateTimeOffset(2020, 2025);

        if (expectedYear == null || expectedMonth is null || expectedDay is null)
        {
            Assert.That(result, Is.Null);
        }
        else
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value.Year, Is.EqualTo(expectedYear.Value));
            Assert.That(result.Value.Month, Is.EqualTo(expectedMonth.Value));
            Assert.That(result.Value.Day, Is.EqualTo(expectedDay.Value));
        }
    }

}