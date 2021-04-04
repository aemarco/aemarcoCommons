using System;
using System.Collections.Generic;


// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools
{
    public static partial class PowerConsole
    {
        /// <summary>
        /// Ensures a Selection in a ConsoleMenu style
        /// </summary>
        /// <param name="header">menu header</param>
        /// <param name="selectable">selectable items</param>
        /// <param name="displayProperty">Displayed as</param>
        /// <returns>Selected item</returns>

        public static T EnsureSelection<T>(string header, IEnumerable<T> selectable, Func<T, string> displayProperty)
            where T : class
        {
            return MenuSelectionHelper(header, selectable, displayProperty, false);
        }

        /// <summary>
        /// Allows Selection in a ConsoleMenu style, adds Abort on the end of menu
        /// </summary>
        /// <param name="header">menu header</param>
        /// <param name="selectable">selectable items</param>
        /// <param name="displayProperty">Displayed as</param>
        /// <returns>Selected item or null on abort</returns>

        public static T AbortableSelection<T>(string header, IEnumerable<T> selectable, Func<T, string> displayProperty)
            where T : class
        {
            return MenuSelectionHelper(header, selectable, displayProperty, true);
        }
    }
}