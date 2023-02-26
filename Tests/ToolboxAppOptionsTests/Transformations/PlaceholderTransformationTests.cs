using aemarcoCommons.ToolboxAppOptions.Transformations;

namespace ToolboxAppOptionsTests.Transformations;
public class PlaceholderTransformationTests : AppOptionTestBase
{

    [Test]
    public void PerformReadTransformation_DoesResolvePlaceholders()
    {
        var transformer = new PlaceholderTransformation();
        var result = transformer.PerformReadTransformation(
            "{{{PlaceholderValue}}}",
            typeof(PlaceholderTestSettings).GetProperty(nameof(PlaceholderTestSettings.Message)),
            Config);
        result.Should().Be("Bob");
    }
    [Test]
    public void PerformWriteTransformation_DoesNothing()
    {
        var transformer = new PlaceholderTransformation();
        var result = transformer.PerformWriteTransformation(
            "Bob",
            typeof(PlaceholderTestSettings).GetProperty(nameof(PlaceholderTestSettings.Message)),
            Config);
        result.Should().Be("Bob");
    }
    [Test]
    public void TransformObject_DoesResolvePlaceholders()
    {
        var config = Config;
        var transformer = new PlaceholderTransformation();
        var testSettings = config
            .GetSection(nameof(PlaceholderTestSettings))
            .Get<PlaceholderTestSettings>()
            ?? throw new NullReferenceException();

        StringTransformerBase.TransformObject(
            testSettings,
            config,
            transformer.PerformReadTransformation);

        testSettings.Message.Should().Be("Bob");
    }
    public class PlaceholderTestSettings : SettingsBase
    {
        public required string Message { get; set; }
    }

}
