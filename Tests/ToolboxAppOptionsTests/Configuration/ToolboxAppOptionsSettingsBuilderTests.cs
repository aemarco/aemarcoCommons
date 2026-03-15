using aemarcoCommons.ToolboxAppOptions.Transformations;
using System.Reflection;

namespace ToolboxAppOptionsTests.Configuration;

public class ToolboxAppOptionsSettingsBuilderTests : AppOptionTestBase
{

    [Test]
    public void AddStringTransformation_Works()
    {

        var builder = new ToolboxAppOptionsSettingsBuilder();
        var transformation = new PlaceholderTransformation();

        _ = builder.AddStringTransformation(transformation);

        var result = builder.Build();
        result.StringTransformations.Count.ShouldBe(1);
        result.StringTransformations[0].ShouldBeSameAs(transformation);
    }

    [Test]
    public void AddAssemblyMarker_Works()
    {
        var builder = new ToolboxAppOptionsSettingsBuilder();
        var targetType = typeof(ToolboxAppOptionsSettingsBuilderTests);
        var targetAssName = targetType.Assembly.FullName;
        var entryAssName = Assembly.GetEntryAssembly()?.FullName;

        _ = builder.AddAssemblyMarker(targetType);

        var result = builder.Build();
        result.Assemblies.Count.ShouldBe(2);
        result.Assemblies[0].FullName.ShouldBe(targetAssName);
        //builder adds entry assembly if not already in there
        result.Assemblies[1].FullName.ShouldBe(entryAssName);
    }

    [Test]
    public void AddAssemblies_Works()
    {
        var builder = new ToolboxAppOptionsSettingsBuilder();
        var targetAss = typeof(ToolboxAppOptionsSettingsBuilderTests).Assembly;
        var entryAssName = Assembly.GetEntryAssembly()?.FullName;

        _ = builder.AddAssemblies(targetAss);

        var result = builder.Build();
        result.Assemblies.Count.ShouldBe(2);
        result.Assemblies[0].FullName.ShouldBe(targetAss.FullName);
        //builder adds entry assembly if not already in there
        result.Assemblies[1].FullName.ShouldBe(entryAssName);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void EnableValidationOnStartup_Works(bool enable)
    {
        var builder = new ToolboxAppOptionsSettingsBuilder();

        _ = builder.EnableValidationOnStartup(enable);

        var result = builder.Build();
        result.EnableValidationOnStartup.ShouldBe(enable);
    }

    [Test]
    public void Build_ShouldNarrowDownConfigurationAssemblies()
    {
        var targetAss = typeof(ToolboxAppOptionsSettingsBuilderTests).Assembly;
        var builder = new ToolboxAppOptionsSettingsBuilder().AddAssemblies(targetAss);

        var result = builder.Build();

        result.ConfigurationAssemblies.Count.ShouldBe(1);
        result.ConfigurationAssemblies[0].FullName.ShouldBe(targetAss.FullName);
    }



    [Test]
    public void Build_ShouldNotTouchDefaults()
    {
        var builder = new ToolboxAppOptionsSettingsBuilder();
        var entryAssName = Assembly.GetEntryAssembly()?.FullName;

        var result = builder.Build();

        result.StringTransformations.ShouldBeEmpty();
        result.Assemblies.Count.ShouldBe(1);
        //builder adds entry assembly if not already in there
        result.Assemblies[0].FullName.ShouldBe(entryAssName);
        result.EnableValidationOnStartup.ShouldBeTrue();
        result.ConfigurationTypes.ShouldBeEmpty();
        result.ConfigurationAssemblies.ShouldBeEmpty();
    }

}