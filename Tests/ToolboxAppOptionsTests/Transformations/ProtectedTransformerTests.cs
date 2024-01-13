using aemarcoCommons.ToolboxAppOptions.Transformations;

namespace ToolboxAppOptionsTests.Transformations;
public class ProtectedTransformerTests : AppOptionTestBase
{
    [Test]
    public void PerformReadTransformation_DoesDecrypt()
    {
        var result = Pt.PerformReadTransformation(
            "Nj4KRDlw1axXf05Kv1dHXk3C3G91VaQVN+Xxt7MJjBMCwNqAsxVgJj/5Im/VbNPB3A==",
            typeof(ProtectedTestSettings).GetProperty(nameof(ProtectedTestSettings.Message))!,
            Config);
        result.Should().Be("Bob");
    }
    [Test]
    public void PerformWriteTransformation_DoesEncrypt()
    {
        var writeMessage = "Since encryption does is not deterministic, we need to do both here";
        var transformer = Pt;
        var pi = typeof(ProtectedTestSettings).GetProperty(nameof(ProtectedTestSettings.Message))!;
        var config = Config;


        var encrypted = transformer.PerformWriteTransformation(
            writeMessage,
            pi,
            config);

        var result = Pt.PerformReadTransformation(
            encrypted,
            pi,
            config);

        result.Should().Be(writeMessage);
    }
    [Test]
    public void TransformObject_DoesResolvePlaceholders()
    {
        var config = Config;
        var transformer = Pt;
        var testSettings = config
                               .GetSection(nameof(ProtectedTestSettings))
                               .Get<ProtectedTestSettings>()
                           ?? throw new NullReferenceException();

        StringTransformerBase.TransformObject(
            testSettings,
            config,
            transformer.PerformReadTransformation);

        testSettings.Message.Should().Be("Bob");
    }
    public class ProtectedTestSettings : ISettingsBase
    {
        [Protected]
        public required string Message { get; set; }
    }









}
