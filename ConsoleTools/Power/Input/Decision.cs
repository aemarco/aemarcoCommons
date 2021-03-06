﻿// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools
{
    public static partial class PowerConsole
    {
        /// /// <summary>
        /// Gets a Yes/No decision through a ConsoleMenu selection
        /// </summary>
        /// /// <param name="question">question to ask</param>
        /// /// <param name="clear">clear console before asking</param>
        /// <returns>true if decision is Yes</returns>
        public static bool EnsureDecision(string question, bool clear = true)
        {
            var selection = MenuSelectionHelper(
                question,
                new[] { "Yes", "No" },
                x => x,
                false,
                clear);
            return selection == "Yes";
        }
    }
}
