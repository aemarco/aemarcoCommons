using Extensions.contentExtensions;
using NUnit.Framework;

namespace ExtensionsTests.contentExtensionsTests
{
    public class AdultExtensionsTests
    {
        //[SetUp]
        //public void Setup()
        //{
        //}



        [TestCase(50, 100, 50)]
        [TestCase(10, 80, 12)]
        [TestCase(22, 60, 37)]
        [TestCase(33, 40, 82)]
        [TestCase(10, 20, 50)]
        [TestCase(5, 10, 50)]
        public void ToDisplayAdult_Converts_Correct(int adultLevel, int userMax, int expected)
        {

            var result = adultLevel.ToDisplayAdult(userMax);
            Assert.AreEqual(expected, result);
        }


        [TestCase(50, 100, 50)]
        [TestCase(12, 80, 10)]
        [TestCase(37, 60, 22)]
        [TestCase(82, 40, 33)]
        [TestCase(50, 20, 10)]
        [TestCase(50, 10, 5)]
        public void ToRealAdult_Converts_Correct(int displayAdult, int userMax, int expected)
        {

            var result = displayAdult.ToRealAdult(userMax);
            Assert.AreEqual(expected, result);
        }
    }
}