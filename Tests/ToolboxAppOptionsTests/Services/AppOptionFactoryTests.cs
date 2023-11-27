// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ToolboxAppOptionsTests.Services;


/// <summary>
/// those tests might be a bit messy, because we test all the factory stuff indirectly.
/// probably that earlier or later should be converted to pure unit tests instead
/// </summary>
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




    [Test]
    public void Validate_DoesPassValidate()
    {
        var settings = Sp.GetRequiredService<ValidationPassTestSettings>();
        settings.Message.Should().Be("Validated");
    }
    public class ValidationPassTestSettings : SettingsBase
    {
        public required string Message { get; set; }
    }
    public class ValidationPassTestSettingsValidator : AbstractValidator<ValidationPassTestSettings>
    {
        public ValidationPassTestSettingsValidator()
        {
            RuleFor(x => x.Message)
                .NotEmpty();
        }
    }


    [Test]
    public void Validate_DoesFailValidate()
    {
        Action action = () => _ = Sp.GetRequiredService<ValidationFailTestSettings>();
        action.Should().Throw<OptionsValidationException>();
    }
    public class ValidationFailTestSettings : SettingsBase
    {
        public required string Message { get; set; }
    }
    public class ValidationFailTestValidator : AbstractValidator<ValidationFailTestSettings>
    {
        public ValidationFailTestValidator()
        {
            RuleFor(x => x.Message)
                .NotEmpty();
        }
    }

}


