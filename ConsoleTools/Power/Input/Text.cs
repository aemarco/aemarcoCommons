using System;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools
{
    public static partial class PowerConsole
    {
        /// <summary>
        /// Gets a text input from the user
        /// </summary>
        /// <param name="prompt">prompt</param>
        /// <param name="minLength">min length</param>
        /// <param name="clear">console should be cleared?</param>
        /// <returns>input string</returns>
        public static string EnsureTextInput(string prompt, int minLength = 1, bool clear = false)
        {
            string input = null;

            while (input == null || input.Length < minLength)
            {
                if (clear) Console.Clear();
                var text = prompt;
                if (input != null && input.Length < minLength)
                {
                    text += $" (minimum {minLength} chars)";
                }
                Console.Write($"{text}: ");
                input = Console.ReadLine();
            }

            return input;
        }
    }
}
