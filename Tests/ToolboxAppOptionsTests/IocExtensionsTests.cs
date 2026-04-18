// ReSharper disable UnusedAutoPropertyAccessor.Global

using aemarcoCommons.ToolboxAppOptions.Transformations;

namespace ToolboxAppOptionsTests;


public class ServiceCollectionExtensionsTests : AppOptionTestBase
{
    [Test]
    public void AddConfigOptionsUtils_ServiceCollectionShowCase()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        var sc = new ServiceCollection()
            .AddConfigOptionsUtils(config, x =>
            {
                x.AddAssemblyMarker(typeof(AppOptionTestBase));
                x.AddStringTransformation(new PlaceholderTransformation());
            });

        //no need for AddOptions with ShowCaseSettings

        var sp = sc.BuildServiceProvider();
        var magic = sp.GetRequiredService<ShowCaseSettings>();

        magic.Message.ShouldBe("awesome");

    }
    [SettingsPath("ShowCase")]
    public class ShowCaseSettings : ISettingsBase
    {
        public required string Message { get; set; }
    }





    [Test]
    public void AddConfigOptionsUtils_RegistersIConfiguration()
    {
        var config = Sp.GetService<IConfiguration>();
        config.ShouldNotBeNull();
    }
    [Test]
    public void AddConfigOptionsUtils_RegistersIConfigurationRoot()
    {
        var config = Sp.GetService<IConfigurationRoot>();
        config.ShouldNotBeNull();
    }
    [Test]
    public void AddConfigOptionsUtils_RegistersConfigurationOptions()
    {
        var config = Sp.GetService<ToolboxAppOptionsSettings>();
        config.ShouldNotBeNull();
    }
    [Test]
    public void AddConfigOptionsUtils_RegistersOptions()
    {
        var settings = Sp
            .GetRequiredService<IOptions<TestSettings>>()
            .Value;
        settings.Message.ShouldBe("HelloWorld");
    }
    [Test]
    public void AddConfigOptionsUtils_RegistersSelf()
    {
        var settings = Sp
            .GetRequiredService<TestSettings>();
        settings.Message.ShouldBe("HelloWorld");
    }
    [Test]
    public void AddConfigOptionsUtils_RegistersValidators()
    {
        var validator = Sp.GetService<IValidator<TestSettings>>();
        validator.ShouldNotBeNull();
    }

    public class TestSettings : ISettingsBase
    {
        public required string Message { get; set; }
    }
    public class TestSettingsValidator : AbstractValidator<TestSettings>;

}


