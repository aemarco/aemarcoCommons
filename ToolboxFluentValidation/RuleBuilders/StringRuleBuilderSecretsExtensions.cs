namespace aemarcoCommons.ToolboxFluentValidation.RuleBuilders;

public static class StringRuleBuilderSecretsExtensions
{
    public static IRuleBuilderOptions<T, string?> BeConfiguredSecret<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(text =>
                text is not null &&
                text != "secret")
            .WithMessage("Secret '{PropertyName}' must be configured");
    }


}