// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools;

public static partial class PowerConsole
{

    [Obsolete("Use EnsureInput(prompt, min, max, clear) instead")]
    public static int EnsureIntInput(string prompt, bool clear = false) =>
        EnsureIntInput(prompt, int.MinValue, int.MaxValue, clear);
    [Obsolete("Use EnsureInput(prompt, min, max, clear) instead")]
    public static int EnsureIntInputInRange(string prompt, int min = int.MinValue, int max = int.MaxValue, bool clear = false) =>
        EnsureIntInput(prompt, min, max, clear);



    [Obsolete("Use EnsureNullableIntInput(prompt, min, max, clear) instead")]
    public static int? EnsureNullableIntInput(string prompt, bool clear = false) =>
        EnsureNullableIntInput(prompt, int.MinValue, int.MaxValue, clear);


    [Obsolete("Use EnsureNullableIntInput(prompt, min, max, clear) instead")]
    public static int? EnsureNullableIntInputInRange(string prompt, int min = int.MinValue, int max = int.MaxValue, bool clear = false) =>
        EnsureNullableIntInput(prompt, min, max, clear);



}
