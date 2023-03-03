using aemarcoCommons.Toolbox;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace ToolboxTests;
public class BootstrapperExtensionsTests
{

    [Test]
    public void SetupToolbox_Works()
    {
        var sc = new ServiceCollection();

        sc.SetupToolbox();


        _ = sc.BuildServiceProvider();


    }


}
