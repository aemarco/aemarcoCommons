using aemarcoCommons.Toolbox.GeoTools;
using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ToolboxTests.GeoTools;

[SingleThreaded]
public class GeoServiceTests
{
    private GeoService _service = null!;
    [SetUp]
    public void Init()
    {
        _service = new GeoService();
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


    #endregion
}