using Extensions.FileExtensions;
using NUnit.Framework;

namespace ExtensionsTests.FileExtensionsTests
{
    public class StorageSizeStuffTests
    {




        //[Test, TestCaseSource("CovertCases")]
        [TestCase(1, "mb", "b", 1048576)]
        [TestCase(5, "kb", "b", 5120)]
        [TestCase(2097152, "kb", "gb", 2)]
        [TestCase(1, "tb", "tb", 1)]
        public void ConvertFromTo_Converts_Correct(long val, string su, string tu, long expected)
        {
            var result = val.ConvertFromTo(su, tu);
            Assert.AreEqual(expected, result);
        }

    }
}
