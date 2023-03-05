using aemarcoCommons.Toolbox;
using aemarcoCommons.Toolbox.Oidc;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace ToolboxTests;
public class BootstrapperExtensionsTests
{

    [Test]
    public void SetupHttpClientStuff_RegistersResolvable_OidcTokenRenewalHandler()
    {
        var sp = new ServiceCollection()
            .SetupHttpClientStuff()
            .BuildServiceProvider();

        var s = sp.GetService<OidcTokenRenewalHandler>();


        s.Should().NotBeNull();

    }
















}
