// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ToolboxConsole;

public static partial class PowerConsole
{
    public static string EnsureTextInput(string prompt, int minLength = 1, bool clear = false)
    {
        if (clear)
            Console.Clear();

        var textPrompt = new TextPrompt<string>($"[purple]{prompt}[/]")
            .PromptStyle("green")
            .Validate(x =>
                x.Length < minLength
                    ? ValidationResult.Error($"[red]Must be at least {minLength} letter(s)[/]")
                    : ValidationResult.Success());
        textPrompt.AllowEmpty = minLength == 0;

        return AnsiConsole.Prompt(textPrompt);
    }

    public static int EnsureIntInput(string prompt, int min = int.MinValue, int max = int.MaxValue, bool clear = false)
    {
        if (clear)
            Console.Clear();

        return AnsiConsole.Prompt(
            new TextPrompt<int>($"[purple]{prompt.EscapeMarkup()}[/]")
                .PromptStyle("green")
                .Validate(number =>
                {
                    if (number < min)
                        return ValidationResult.Error($"Number must be equal or greater than {min}.");
                    if (number > max)
                        return ValidationResult.Error($"Number must be smaller or equal than {max}.");
                    return ValidationResult.Success();
                }));
    }

    public static int? EnsureNullableIntInput(string prompt, int min = int.MinValue, int max = int.MaxValue, bool clear = false)
    {
        if (clear)
            Console.Clear();

        return AnsiConsole.Prompt(
            new TextPrompt<int?>($"[purple]{prompt}[/]")
                .PromptStyle("green")
                .AllowEmpty()
                .DefaultValue(null)
                .ShowDefaultValue(false)
                .Validate(number =>
                {
                    if (number < min)
                        return ValidationResult.Error($"Number must be equal or greater than {min}.");
                    if (number > max)
                        return ValidationResult.Error($"Number must be smaller or equal than {max}.");
                    return ValidationResult.Success();
                }));
    }

    public static bool EnsureDecision(string question, bool clear = true)
    {
        if (clear)
            Console.Clear();

        static string GetText(bool x) => x ? "Yes" : "No";
        var result = AnsiConsole.Prompt(
            new SelectionPrompt<bool>()
                .Title($"[purple]{question}[/]")
                .UseConverter(GetText)
                .AddChoices([true, false]));
        AnsiConsole.MarkupLine($"[purple]{question}[/] [green]{GetText(result)}[/]");

        return result;
    }
}
