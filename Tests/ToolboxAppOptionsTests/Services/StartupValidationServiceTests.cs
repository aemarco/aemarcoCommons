using Microsoft.Extensions.Hosting;
using System.Linq;

namespace ToolboxAppOptionsTests.Services;
public class StartupValidationServiceTests : AppOptionTestBase
{

    //there is already a failing validation in other tests, so... check multiple here
    [Test]
    public void StartupValidationService_ValidatingOnStartup()
    {
        var app = Host.CreateApplicationBuilder();
        app.Services.AddConfigOptionsUtils(app.Configuration);
        var host = app.Build();

        try
        {
            host.Start();
        }
        catch (AggregateException ex)
        {
            ex.InnerExceptions.Should().AllBeOfType<OptionsValidationException>();
        }
    }


    [Test]
    public void StartupValidationService_ValidatingOnStartupSingle()
    {
        var app = Host.CreateApplicationBuilder();
        app.Services.AddConfigOptionsUtils(app.Configuration, x => x.AddAssemblyMarker(typeof(AppOptionTestBase)));
        //we strip all validators, so only the Multi... is failing,
        //and therefor we get only a single exception then in StartupValidationService
        foreach (var service in app.Services
                     .Where(x => x.ServiceType.IsAssignableTo(typeof(IValidator)))
                     .Where(x => x.ServiceType != typeof(IValidator<SingleValidationTestSettings>))
                     .ToList())
        {
            app.Services.Remove(service);
        }
        var host = app.Build();


        FluentActions.Invoking(() => host.Start())
            .Should().Throw<OptionsValidationException>();
    }
    public class SingleValidationTestSettings : SettingsBase
    {
        public required string Message { get; set; }
    }
    public class SingleValidationTestSettingsValidator : AbstractValidator<SingleValidationTestSettings>
    {
        public SingleValidationTestSettingsValidator()
        {
            RuleFor(x => x.Message)
                .Must(x => x == "OtherThanHelloWorld");
        }
    }

}
