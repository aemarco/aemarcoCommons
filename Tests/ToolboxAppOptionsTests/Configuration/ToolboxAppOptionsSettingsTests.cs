namespace ToolboxAppOptionsTests.Configuration;

public class ToolboxAppOptionsSettingsTests : AppOptionTestBase
{

    [Test]
    public void HasExpectedDefaults()
    {
        var result = new ToolboxAppOptionsSettings();

        result.StringTransformations.ShouldBeEmpty();
        result.Assemblies.ShouldBeEmpty();
        result.EnableValidationOnStartup.ShouldBeTrue();

        result.ConfigurationTypes.ShouldBeEmpty();
        result.ConfigurationAssemblies.ShouldBeEmpty();
    }
}