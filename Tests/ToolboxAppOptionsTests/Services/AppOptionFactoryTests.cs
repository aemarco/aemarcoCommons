// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ToolboxAppOptionsTests.Services;
public class AppOptionFactoryTests : AppOptionTestBase
{

    [Test]
    public void Configure_DoesSettingsPath_WithRoot()
    {
        var settings = Sp.GetRequiredService<RootTestSettings>();
        settings.Message.Should().Be("root");
    }
    [SettingsPath("")]
    public class RootTestSettings : SettingsBase
    {
        public required string Message { get; set; }
    }


    [Test]
    public void Configure_DoesSettingsPath_WithClass()
    {
        var settings = Sp.GetRequiredService<ClassSettings>();
        settings.Message.Should().Be("class");
    }
    [SettingsPath("ClassSettings")]
    public class ClassSettings : SettingsBase
    {
        public required string Message { get; set; }
    }


    [Test]
    public void Configure_DoesSettingsPath_Nested()
    {
        var settings = Sp.GetRequiredService<NestedTestSettings>();
        settings.Message.Should().Be("nested");
    }
    [SettingsPath("NestedA:NestedB")]
    public class NestedTestSettings : SettingsBase
    {
        public required string Message { get; set; }
    }



    [Test]
    public void PostConfigure_DoesStringTransformations()
    {
        var settings = Sp.GetRequiredService<PlaceholderTestSettings>();
        settings.Message.Should().Be("Bob");
    }
    public class PlaceholderTestSettings : SettingsBase
    {
        public required string Message { get; set; }
    }







}


