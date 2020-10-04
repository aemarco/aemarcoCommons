using System;
using System.Net.Http;
using System.Threading.Tasks;
using aemarcoCommons.Toolbox.GeoTools;
using FluentAssertions;
using NUnit.Framework;

namespace ToolboxTests.GeoTools
{

    [SingleThreaded]
    public class GeoServiceTests
    {
        private GeoService _service;
        [SetUp]
        public void Init()
        {
            _service = new GeoService(new HttpClient())
            {
                NumberOfCachedSunriseSunsetInfos = 2,
                MinIntervalOfIpInfoUpdate = TimeSpan.FromMinutes(1)
            };
        }

        #region IpInfo

        [Test]
        public async Task GetIpInfo_Delivers()
        {
            var result = await _service.GetIpInfo();
            result.Should().NotBeNull().And.Contain(".");
        }

        [Test]
        public async Task GetIpInfo_DeliversTwoTimesSameObject()
        {
            var result1 = await _service.GetIpInfo();
            var result2 = await _service.GetIpInfo();
            result1.Should().BeSameAs(result2); //compare reference
        }

        #endregion

        #region SunriseSunset

        [Test]
        public async Task GetSunriseSunsetInfo_Delivers()
        {
            var result = await _service.GetSunriseSunsetInfoInfo(36.7201600f, -4.4203400f);
            result.Should().NotBeNull();
        }

        [Test]
        public async Task GetSunriseSunsetInfo_DeliversTwoTimesSameObject()
        {
            var result1 = await _service.GetSunriseSunsetInfoInfo(36.7201600f, -4.4203400f);
            var result2 = await _service.GetSunriseSunsetInfoInfo(36.7201600f, -4.4203400f);
            result1.Should().BeSameAs(result2); //compare reference
        }

        [Test]
        public async Task GetSunriseSunsetInfo_ReturnsNull()
        {
            var result = await _service.GetSunriseSunsetInfoInfo(0f, 0f);
            result.Should().BeNull();
        }

        
        [Test]
        public void GetSunriseSunsetInfo_Throws()
        {
            _service.Invoking(async x => await x.GetSunriseSunsetInfoInfo(0f, 0f, throwExceptions: true))
                .Should()
                .Throw<Exception>();
        }

        #endregion
    }
}
