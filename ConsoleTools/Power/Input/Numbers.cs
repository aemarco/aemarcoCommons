// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools;

public static partial class PowerConsole
{

    /// <summary>
    /// Ensures input of an integer number from the user, which lies within given range
    /// </summary>
    /// <param name="prompt">prompt</param>
    /// <param name="min">inclusive minimum of the range</param>
    /// <param name="max">inclusive maximum of the range</param>
    /// <param name="clear">console should be cleared?</param>
    /// <returns>input number in range</returns>
    public static int EnsureIntInput(string prompt, int min = int.MinValue, int max = int.MaxValue, bool clear = false)
    {
        if (clear)
            Console.Clear();

        var result = AnsiConsole.Prompt(
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
        return result;
    }


    /// <summary>
    /// Ensures input of either null or an integer from the user, which lies within given range
    /// </summary>
    /// <param name="prompt">prompt</param>
    /// <param name="min">inclusive minimum of the range</param>
    /// <param name="max">inclusive maximum of the range</param>
    /// <param name="clear">console should be cleared?</param>
    /// <returns>nullable input number in range</returns>
    public static int? EnsureNullableIntInput(string prompt, int min = int.MinValue, int max = int.MaxValue, bool clear = false)
    {
        if (clear)
            Console.Clear();

        var result = AnsiConsole.Prompt(
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
        return result;
    }

}