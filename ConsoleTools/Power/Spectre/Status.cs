using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools;

public static partial class PowerConsole
{
    public static async Task<T> StartStatusAsync<T>(
        Func<StatusContext, Task<T>> work)
    {
        return await AnsiConsole.Status()
            .SpinnerStyle(Style.Parse("green"))
            .StartAsync(
                "[purple]Doing stuff...[/]",
                work);
    }
}
