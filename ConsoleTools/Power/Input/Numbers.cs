// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools;

public static partial class PowerConsole
{

    #region int

    /// <summary>
    /// Ensures input of a integer number from the user
    /// </summary>
    /// <param name="prompt">prompt</param>
    /// <param name="clear">console should be cleared?</param>
    /// <returns>input number</returns>
    public static int EnsureIntInput(string prompt, bool clear = false)
    {
        int result;
        string input;
        do
        {
            input = EnsureTextInput(prompt, 1, clear);
        }
        while (!int.TryParse(input, out result));
        return result;
    }

    /// <summary>
    /// Ensures input of a integer number from the user, which lies within given range
    /// </summary>
    /// <param name="prompt">prompt</param>
    /// <param name="min">inclusive minimum of the range</param>
    /// <param name="max">inclusive maximum of the range</param>
    /// <param name="clear">console should be cleared?</param>
    /// <returns>input number in range</returns>
    public static int EnsureIntInputInRange(string prompt, int min = 0, int max = int.MaxValue, bool clear = false)
    {
        int number;
        do
        {
            number = EnsureIntInput(prompt, clear);

        } while (number < min || number > max);
        return number;
    }

    #endregion

    #region int?

    /// <summary>
    /// Ensures input of either null or a integer from the user
    /// </summary>
    /// <param name="prompt">prompt</param>
    /// <param name="clear">console should be cleared?</param>
    /// <returns>nullable input number</returns>
    public static int? EnsureNullableIntInput(string prompt, bool clear = false)
    {
        int result;
        string input;
        do
        {
            input = EnsureTextInput(prompt, 0, clear);
            if (string.IsNullOrEmpty(input)) return null;

        } while (!int.TryParse(input, out result));
        return result;
    }

    /// <summary>
    /// Ensures input of either null or a integer from the user, which lies within given range
    /// </summary>
    /// <param name="prompt">prompt</param>
    /// <param name="min">inclusive minimum of the range</param>
    /// <param name="max">inclusive maximum of the range</param>
    /// <param name="clear">console should be cleared?</param>
    /// <returns>nullable input number in range</returns>
    public static int? EnsureNullableIntInputInRange(string prompt, int min = 0, int max = int.MaxValue, bool clear = false)
    {
        int? result;
        do
        {
            result = EnsureNullableIntInput(prompt, clear);
            if (!result.HasValue) return null;

        } while (result.Value < min || result.Value > max);
        return result;
    }

    #endregion

}