using aemarcoCommons.ToolboxAppOptions.Transformations;
using System.Reflection;

namespace ToolboxAppOptionsTests.Configuration;

public class ToolboxAppOptionsSettingsBuilderTests : AppOptionTestBase
{

    [Test]
    public void AddStringTransformation_Works()
    {

        var builder = new ConfigurationOptionsBuilder();
        var transformation = new PlaceholderTransformation();

        _ = builder.AddStringTransformation(transformation);

        var result = builder.Build();
        result.StringTransformations.Should().HaveCount(1);
        result.StringTransformations[0].Should().BeSameAs(transformation);
    }

    [Test]
    public void AddAssemblyMarker_Works()
    {
        var builder = new ConfigurationOptionsBuilder();
        var targetType = typeof(ToolboxAppOptionsSettingsBuilderTests);
        var targetAssName = targetType.Assembly.FullName;
        var entryAssName = Assembly.GetEntryAssembly()?.FullName;

        _ = builder.AddAssemblyMarker(targetType);

        var result = builder.Build();
        result.Assemblies.Should().HaveCount(2);
        result.Assemblies[0].Should().Match(x => x.FullName == targetAssName);
        //builder adds entry assembly if not already in there
        result.Assemblies[1].Should().Match(x => x.FullName == entryAssName);
    }

    [Test]
    public void AddAssemblies_Works()
    {
        var builder = new ConfigurationOptionsBuilder();
        var targetAss = typeof(ToolboxAppOptionsSettingsBuilderTests).Assembly;
        var entryAssName = Assembly.GetEntryAssembly()?.FullName;

        _ = builder.AddAssemblies(targetAss);

        var result = builder.Build();
        result.Assemblies.Should().HaveCount(2);
        result.Assemblies[0].Should().Match(x => x.FullName == targetAss.FullName);
        //builder adds entry assembly if not already in there
        result.Assemblies[1].Should().Match(x => x.FullName == entryAssName);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void EnableValidationOnStartup_Works(bool enable)
    {
        var builder = new ConfigurationOptionsBuilder();

        _ = builder.EnableValidationOnStartup(enable);

        var result = builder.Build();
        result.EnableValidationOnStartup.Should().Be(enable);
    }

    [Test]
    public void Build_ShouldNarrowDownConfigurationAssemblies()
    {
        var targetAss = typeof(ToolboxAppOptionsSettingsBuilderTests).Assembly;
        var builder = new ConfigurationOptionsBuilder().AddAssemblies(targetAss);

        var result = builder.Build();

        result.ConfigurationAssemblies.Should().HaveCount(1);
        result.ConfigurationAssemblies[0].Should().Match(x => x.FullName == targetAss.FullName);
    }



    [Test]
    public void Build_ShouldNotTouchDefaults()
    {
        var builder = new ConfigurationOptionsBuilder();
        var entryAssName = Assembly.GetEntryAssembly()?.FullName;

        var result = builder.Build();

        result.StringTransformations.Should().HaveCount(0);
        result.Assemblies.Should().HaveCount(1);
        //builder adds entry assembly if not already in there
        result.Assemblies[0].Should().Match(x => x.FullName == entryAssName);
        result.EnableValidationOnStartup.Should().Be(true);
        result.ConfigurationTypes.Should().HaveCount(0);
        result.ConfigurationAssemblies.Should().HaveCount(0);
    }

}