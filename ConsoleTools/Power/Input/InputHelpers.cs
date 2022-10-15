using System;
using System.Collections.Generic;


// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools;

public static partial class PowerConsole
{
    private static T MenuSelectionHelper<T>(string header, IEnumerable<T> selectable, Func<T, string> displayProperty, bool abortable, bool clear = true)
        where T : class
    {
        T result = null;

        if (clear) Console.Clear();
        var items = new List<ConsoleMenuItem>();

        // ReSharper disable once LoopCanBeConvertedToQuery
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
            }, null));
        }

        var menu = new ConsoleMenu(header, items);
        menu.RunConsoleMenu();

        return result;
    }

}