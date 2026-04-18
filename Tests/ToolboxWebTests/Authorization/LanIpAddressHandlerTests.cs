using aemarcoCommons.ToolboxWeb.Authorization;
using System.Net;

namespace ToolboxWebTests.Authorization;

public class LanIpAddressHandlerTests
{

    // ParseSubnets
    [TestCase("10.0.0.0/8", "10.0.0.0", 8)]
    [TestCase("172.16.0.0/12", "172.16.0.0", 12)]
    [TestCase("192.168.0.0/16", "192.168.0.0", 16)]
    [TestCase("192.168.1.0/24", "192.168.1.0", 24)]
    [TestCase("2001:db8::/32", "2001:db8::", 32)]
    public void ParseSubnets_ValidCidr_ReturnsParsedEntry(string cidr, string expectedNetwork, int expectedPrefix)
    {
        var result = LanIpAddressHandler.ParseSubnets([cidr]);

        result.Count.ShouldBe(1);
        var (network, prefix) = result[0];
        network.ShouldBe(IPAddress.Parse(expectedNetwork));
        prefix.ShouldBe(expectedPrefix);
        result.Dump();
    }

    [TestCase(new[] { "10.0.0.0/8", "172.16.0.0/12", "192.168.0.0/16" }, 3)] // all valid
    [TestCase(new[] { "not-a-subnet", "192.168.0.0/16" }, 1)]               // invalid skipped
    [TestCase(new string[0], 0)]                                              // empty
    public void ParseSubnets_ReturnsExpectedCount(string[] input, int expectedCount)
    {
        var result = LanIpAddressHandler.ParseSubnets([.. input]);

        result.Count.ShouldBe(expectedCount);
        result.Dump();
    }




    // IsInSubnet
    [TestCase("192.168.1.50", "192.168.0.0", 16, true)]
    [TestCase("192.168.20.1", "192.168.0.0", 16, true)]
    [TestCase("192.168.0.0", "192.168.0.0", 16, true)]   // network address itself
    [TestCase("192.168.255.255", "192.168.0.0", 16, true)] // broadcast
    [TestCase("10.0.0.1", "192.168.0.0", 16, false)]
    [TestCase("10.20.30.40", "10.0.0.0", 8, true)]
    [TestCase("11.0.0.1", "10.0.0.0", 8, false)]
    [TestCase("172.16.5.1", "172.16.0.0", 12, true)]
    [TestCase("172.32.0.1", "172.16.0.0", 12, false)]
    [TestCase("192.168.1.1", "192.168.1.0", 24, true)]
    [TestCase("192.168.2.1", "192.168.1.0", 24, false)]
    [TestCase("::1", "192.168.0.0", 16, false)] //mixed network families
    public void IsInSubnet_MatchesSingular_AsExpected(string address, string network, int prefix, bool expected)
    {
        var ipAddress = IPAddress.Parse(address);
        var ipNetwork = IPAddress.Parse(network);

        var result = LanIpAddressHandler.IsInSubnet(ipAddress, ipNetwork, prefix);

        result.ShouldBe(expected);
        result.Dump();
    }


    [TestCase("192.168.5.100", "192.168.0.0/16", true)]
    [TestCase("8.8.8.8", "192.168.0.0/16", false)]
    [TestCase("192.168.1.1", null, false)] //empty => false
    public void IsInSubnet_MatchesMultiple_AsExpected(string ip, string? cidr, bool expected)
    {
        var ipAddress = IPAddress.Parse(ip);
        var subnets = cidr is null
            ? LanIpAddressHandler.ParseSubnets([])
            : LanIpAddressHandler.ParseSubnets([cidr]);

        var result = LanIpAddressHandler.IsInSubnet(ipAddress, subnets);

        result.ShouldBe(expected);
        result.Dump();
    }

    [Test]
    public void IsInSubnet_MatchesSecondSubnetWhenFirstFails()
    {
        var ipAddress = IPAddress.Parse("192.168.20.5");
        var subnets = LanIpAddressHandler.ParseSubnets(["10.0.0.0/8", "192.168.0.0/16"]);

        var result = LanIpAddressHandler.IsInSubnet(ipAddress, subnets);

        result.ShouldBeTrue();
        result.Dump();
    }

}
