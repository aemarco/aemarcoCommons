// ReSharper disable UnusedAutoPropertyAccessor.Global

using aemarcoCommons.ToolboxAppOptions.Transformations;

namespace ToolboxAppOptionsTests;


public class IocExtensionsTests : AppOptionTestBase
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
                x.AddStringTransformation(new PlaceholderTransformation());
            });

        //no need for AddOptions with ShowCaseSettings

        var sp = sc.BuildServiceProvider();
        var magic = sp.GetRequiredService<ShowCaseSettings>();

        magic.Message.Should().Be("awesome");

    }
    [SettingsPath("ShowCase")]
    public class ShowCaseSettings : SettingsBase
    {
        public required string Message { get; set; }
    }





    [Test]
    public void AddConfigOptionsUtils_RegistersIConfiguration()
    {
        var config = Sp.GetService<IConfiguration>();
        config.Should().NotBeNull();
    }
    [Test]
    public void AddConfigOptionsUtils_RegistersIConfigurationRoot()
    {
        var config = Sp.GetService<IConfigurationRoot>();
        config.Should().NotBeNull();
    }
    [Test]
    public void AddConfigOptionsUtils_RegistersConfigurationOptions()
    {
        var config = Sp.GetService<ConfigurationOptions>();
        config.Should().NotBeNull();
    }
    [Test]
    public void AddConfigOptionsUtils_RegistersOptions()
    {
        var settings = Sp
            .GetRequiredService<IOptions<TestSettings>>()
            .Value;
        settings.Message.Should().Be("HelloWorld");
    }
    [Test]
    public void AddConfigOptionsUtils_RegistersSelf()
    {
        var settings = Sp
            .GetRequiredService<TestSettings>();
        settings.Message.Should().Be("HelloWorld");
    }
    [Test]
    public void AddConfigOptionsUtils_RegistersValidators()
    {
        var validator = Sp.GetService<IValidator<TestSettings>>();
        validator.Should().NotBeNull();
    }

    public class TestSettings : SettingsBase
    {
        public required string ShowCaseMessage { get; set; }
        public required string Message { get; set; }
    }
    public class TestSettingsValidator : AbstractValidator<TestSettings> { }

}


