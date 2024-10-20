namespace ToolboxAppOptionsTests.Configuration;

public class ToolboxAppOptionsSettingsTests : AppOptionTestBase
{

    [Test]
    public void HasExpectedDefaults()
    {
        var result = new ConfigurationOptions();

        result.StringTransformations.Should().HaveCount(0);
        result.Assemblies.Should().HaveCount(0);
        result.EnableValidationOnStartup.Should().Be(true);

        result.ConfigurationTypes.Should().HaveCount(0);
        result.ConfigurationAssemblies.Should().HaveCount(0);
    }
}