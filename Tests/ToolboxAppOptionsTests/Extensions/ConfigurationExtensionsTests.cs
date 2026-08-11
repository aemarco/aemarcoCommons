// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace ToolboxAppOptionsTests.Extensions;

[TestFixture]
internal class ConfigurationExtensionsTests
{

    [Test]
    public void GetResolved_BoolProperty_ReturnsBoundValue()
    {
        var config = BuildConfig(("GetResolvedTestSettings:Enabled", "true"));
        var result = config.GetResolved<GetResolvedTestSettings, bool>(x => x.Enabled);
        result.ShouldBeTrue();
    }
    [Test]
    public void GetResolved_StringProperty_ReturnsBoundValue()
    {
        var config = BuildConfig(("GetResolvedTestSettings:Name", "SomeText"));
        var result = config.GetResolved<GetResolvedTestSettings, string?>(x => x.Name);
        result.ShouldBe("SomeText");
    }
    [Test]
    public void GetResolved_StringPropertyWithPlaceholder_ResolvesPlaceholder()
    {
        var config = BuildConfig(
            ("GetResolvedTestSettings:Name", "Hello {{{PlaceholderValue}}}!"),
            ("PlaceholderValue", "World"));
        var result = config.GetResolved<GetResolvedTestSettings, string?>(x => x.Name);
        result.ShouldBe("Hello World!");
    }
    [Test]
    public void GetResolved_KeyMissing_ReturnsDefault()
    {
        var config = BuildConfig();
        var result = config.GetResolved<GetResolvedTestSettings, bool>(x => x.Enabled);
        result.ShouldBeFalse();
    }

    [Test]
    public void GetResolved_NestedMemberAccess_Throws()
    {
        var config = BuildConfig();
        Should.Throw<ArgumentException>(
            () => config.GetResolved<GetResolvedTestSettings, int>(x => x.Name!.Length));
    }

    [Test]
    public void GetResolved_NotMemberAccess_Throws()
    {
        var config = BuildConfig();
        Should.Throw<ArgumentException>(
            () => config.GetResolved<GetResolvedTestSettings, bool>(x => !x.Enabled));
    }
    private class GetResolvedTestSettings : ISettingsBase
    {
        public bool Enabled { get; set; }
        public string? Name { get; set; }
    }


    [Test]
    public void GetResolved_SettingsPathOverride_ReturnsBoundValue()
    {
        var config = BuildConfig(("Custom:Nested:Enabled", "true"));

        var result = config.GetResolved<GetResolvedPathTestSettings, bool>(x => x.Enabled);

        result.ShouldBeTrue();
    }
    [SettingsPath("Custom:Nested")]
    private class GetResolvedPathTestSettings : ISettingsBase
    {
        public bool Enabled { get; set; }
    }

    [Test]
    public void GetResolved_RootSettingsPath_ReturnsBoundValue()
    {
        var config = BuildConfig(("RootFlag", "true"));
        var result = config.GetResolved<GetResolvedRootTestSettings, bool>(x => x.RootFlag);
        result.ShouldBeTrue();
    }
    [SettingsPath("")]
    private class GetResolvedRootTestSettings : ISettingsBase
    {
        public bool RootFlag { get; set; }
    }




    [Test]
    public void GetResolved_RawPathBool_ReturnsBoundValue()
    {
        var config = BuildConfig(("Auth:Enabled", "true"));
        var result = config.GetResolved<bool>("Auth:Enabled");
        result.ShouldBeTrue();
    }

    [Test]
    public void GetResolved_RawPathString_ReturnsBoundValue()
    {
        var config = BuildConfig(("Auth:Username", "aemarco"));
        var result = config.GetResolved<string?>("Auth:Username");
        result.ShouldBe("aemarco");
    }

    [Test]
    public void GetResolved_RawPathStringWithPlaceholder_ResolvesPlaceholder()
    {
        var config = BuildConfig(
            ("Auth:Username", "{{{PlaceholderValue}}}"),
            ("PlaceholderValue", "aemarco"));
        var result = config.GetResolved<string?>("Auth:Username");
        result.ShouldBe("aemarco");
    }

    [Test]
    public void GetResolved_RawPathMissing_ReturnsDefault()
    {
        var config = BuildConfig();
        var result = config.GetResolved<string?>("Auth:Username");
        result.ShouldBeNull();
    }


    private static IConfigurationRoot BuildConfig(params (string Key, string? Value)[] values)
    {
        var data = values.ToDictionary(x => x.Key, x => x.Value);
        return new ConfigurationBuilder()
            .AddInMemoryCollection(data)
            .Build();
    }

}