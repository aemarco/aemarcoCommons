using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ToolboxConsole;

public static partial class PowerConsole
{
    /// <summary>
    /// Ensures a selection via Spectre SelectionPrompt
    /// </summary>
    public static T EnsureSelection<T>(string header, IEnumerable<T> selectable, Func<T, string> displayProperty)
        where T : class
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<T>()
                .Title($"[purple]{header}[/]")
                .UseConverter(displayProperty)
                .AddChoices([.. selectable]));
    }

    /// <summary>
    /// Allows selection via ConsoleMenu, adds Abort at the end — returns null on abort
    /// </summary>
    public static T? AbortableSelection<T>(string header, IEnumerable<T> selectable, Func<T, string> displayProperty)
        where T : class
    {
        return MenuSelectionHelper(header, selectable, displayProperty, abortable: true);
    }

    private static T? MenuSelectionHelper<T>(string header, IEnumerable<T> selectable, Func<T, string> displayProperty, bool abortable, bool clear = true)
        where T : class
    {
        T? result = null;

        if (clear) Console.Clear();
        var items = new List<ConsoleMenuItem>();

        foreach (var item in selectable)
        {
            var temp = item;
            items.Add(new ConsoleMenuItem<T>(displayProperty(temp), x =>
            {
                result = x;
            }, temp));
        }

        if (abortable)
        {
            items.Add(new ConsoleMenuSeparator());
            items.Add(new ConsoleMenuItem<T>("Abort", x =>
            {
                result = x;
            }));
        }

        var menu = new ConsoleMenu(header, items);
        menu.RunConsoleMenu();

        return result;
    }
}
