using aemarcoCommons.Toolbox;
using aemarcoCommons.Toolbox.Oidc;

namespace ToolboxTests;

public class BootstrapperExtensionsTests
{

    [Test]
    public void SetupHttpClientStuff_RegistersResolvable_OidcTokenRenewalHandler()
    {
        var sp = new ServiceCollection()
            .SetupToolbox()
            .BuildServiceProvider();

        var s = sp.GetService<OidcTokenRenewalHandler>();


        s.ShouldNotBeNull();

    }
















}