
// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ToolboxFluentValidation;

//https://docs.fluentvalidation.net/en/latest/custom-validators.html
public static class StringRuleBuilderExtensions
{

    public static IRuleBuilderOptions<T, string?> BeValidAbsoluteUri<T>(
        this IRuleBuilder<T, string?> ruleBuilder, bool? trailingSlash = false)
    {
        return ruleBuilder
            .Must(text => IsAbsoluteWebUri(text, trailingSlash))
            .WithMessage(
                trailingSlash.HasValue
                    ? trailingSlash.Value
                        ? "'{PropertyName}' must be a valid Web-Uri with a trailing slash"
                        : "'{PropertyName}' must be a valid Web-Uri without a trailing slash"
                    : "'{PropertyName}' must be a valid Web-Uri");
    }



    public static IRuleBuilderOptions<T, string> BeValidAbsoluteWebUri<T>(
        this IRuleBuilder<T, string> ruleBuilder, bool allowTrailingSlash = false)
    {
        return ruleBuilder
            .Must(text => IsAbsoluteWebUri(text, allowTrailingSlash))
            .WithMessage(
                allowTrailingSlash
                ? "'{PropertyName}' must be a valid Web-Uri"
                : "'{PropertyName}' must be a valid Web-Uri without a trailing slash");
    }

    public static IRuleBuilderOptions<T, string?> BeWithoutTrailingSlash<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(text => text is null || !text.EndsWith('/'))
            .WithMessage("'{PropertyName}' must not end with /");
    }



    private static bool IsAbsoluteWebUri(string? url, bool? trailingSlash) =>
        !string.IsNullOrWhiteSpace(url) &&
        (!trailingSlash.HasValue ||
         (trailingSlash.Value && url.EndsWith('/')) ||
         (!trailingSlash.Value && !url.EndsWith('/'))) &&
        Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
        (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);



    public static IRuleBuilderOptions<T, string> BeValidCronExpression<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("'{PropertyName}' must not be an empty cron expression")
            .Must(IsValidCron)
            .WithMessage("'{PropertyName}' must be a valid cron expression");
    }

    private static bool IsValidCron(string cron)
    {
        try
        {
            _ = CronExpressionDescriptor.ExpressionDescriptor.GetDescription(cron);
            return true;
        }
        catch
        {
            return false;
        }
    }






}

//public class PersonValidator : AbstractValidator<Person> {
//    public PersonValidator() {
//        RuleFor(x => x.Pets).Custom((list, context) => {
//            if(list.Count > 10) {
//                context.AddFailure("The list must contain 10 items or fewer");
//            }
//        });
//    }
//}
