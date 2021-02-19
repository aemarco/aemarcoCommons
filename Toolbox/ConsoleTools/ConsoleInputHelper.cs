using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aemarcoCommons.Toolbox.ConsoleTools
{
    public static class ConsoleInputHelper
    {



        /// /// <summary>
        /// Allows Yes/No selection in ConsoleMenu
        /// </summary>
        /// /// <param name="question">question to ask</param>
        /// /// <param name="clear">clear console before asking</param>
        /// <returns>true if decision is Yes</returns>
        public static bool Decision(string question, bool clear = true)
        {
            var selection = MenuSelectionHelper(
                question,
                new string[] { "Yes", "No" },
                x => x,
                false,
                clear);
            return selection == "Yes";
        }

        /// <summary>
        /// Textinput
        /// </summary>
        /// <param name="prompt">prompt</param>
        /// <param name="minLenght">min length</param>
        /// <param name="clear">console should be cleared?</param>
        /// <returns>input string</returns>
        public static string EnsureInput(string prompt, int minLenght = 1, bool clear = false)
        {
            string input = null;

            while (input == null || input.Length < minLenght)
            {
                if (clear) Console.Clear();
                var text = prompt;
                if (input != null && input.Length < minLenght)
                {
                    text += $" (minimum {minLenght} chars)";
                }
                Console.Write($"{text}: ");
                input = Console.ReadLine();
            }

            return input;
        }


        public static int EnsureIntInputInRange(string prompt, int min = 0, int max = int.MaxValue, bool clear = false)
        {
            int? result = null;

            while (result == null || result.Value < min || result.Value > max)
            {
                if (int.TryParse(EnsureInput(prompt, 1, clear), out int newNum))
                    result = newNum;
            }

            return result.Value;
        }

        public static int? EnsureIntInputNullOrInRange(string prompt, int min = 0, int max = int.MaxValue, bool clear = false)
        {
            int? result = null;

            if (Decision($"Want to define {prompt}?", false))
            {
                while (result == null || result.Value < min || result.Value > max)
                {
                    if (int.TryParse(EnsureInput(prompt, 1, clear), out int newNum))
                        result = newNum;
                }
            }

            return result;
        }






        /// <summary>
        /// Allows Selection in a ConsoleMenu
        /// </summary>
        /// <param name="header">menu header</param>
        /// <param name="selectable">selectable items</param>
        /// <param name="displayProperty">Displayed as</param>
        /// <returns>Selected item</returns>
        public static T Selection<T>(string header, IEnumerable<T> selectable, Func<T, string> displayProperty)
            where T : class
        {
            return MenuSelectionHelper(header, selectable, displayProperty, false);
        }
        /// <summary>
        /// Allows Selection in a ConsoleMenu, adds Abort on the end of menu
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
        //internal handling
        private static T MenuSelectionHelper<T>(string header, IEnumerable<T> selectable, Func<T, string> displayProperty, bool abortable, bool clear = true)
            where T : class
        {
            T result = null;
            bool aborted = false;

            while (result == null && !aborted)
            {
                if (clear)
                    Console.Clear();
                else
                    Console.WriteLine();

                List<ConsoleMenuItem> items = new List<ConsoleMenuItem>();
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
                    items.Add(new ConsoleMenuSeperator('-'));
                    items.Add(new ConsoleMenuItem<T>("Abort", x =>
                    {
                        aborted = true;
                    }, null));
                }

                var menu = new ConsoleMenu<T>(header, items);
                menu.RunConsoleMenu();
            }

            return result;
        }



        /// <summary>
        /// Selection of a path in a ConsoleMenu
        /// </summary>
        /// <param name="path">path to start at</param>
        /// <returns>selected path</returns>
        public static string PathSelector(string path)
        {
            var dir = new DirectoryInfo(path);
            if (!dir.Exists) throw new DirectoryNotFoundException($"{path}");

            Console.Clear();
            var dirItems = new List<ConsoleMenuItem>
            {
                new ConsoleMenuItem<DirectoryInfo>("..", x =>
                {
                    path = x != null ? PathSelector(x.FullName) : DriveSelector();
                }, dir.Parent)
            };

            foreach (var sub in dir.GetDirectories().Where(x =>
                !x.Attributes.HasFlag(FileAttributes.Hidden)
                ))
            {
                var temp = sub;
                dirItems.Add(new ConsoleMenuItem<DirectoryInfo>(temp.Name, x =>
                {
                    path = PathSelector(temp.FullName);
                }, temp));
            }
            dirItems.Add(new ConsoleMenuSeperator('-'));

            dirItems.Add(new ConsoleMenuItem<DirectoryInfo>("Select", x =>
            {
                path = dir.FullName;

            }, dir));

            var pMenu = new ConsoleMenu<DirectoryInfo>(dir.FullName, dirItems);
            pMenu.RunConsoleMenu();

            return path;
        }
        //internal handling
        private static string DriveSelector()
        {
            string path = null;
            Console.Clear();

            var driveItems = new List<ConsoleMenuItem<DriveInfo>>();
            foreach (var drive in DriveInfo.GetDrives()
                .Where(x => x.IsReady))
            {
                var temp = drive;
                driveItems.Add(new ConsoleMenuItem<DriveInfo>(temp.Name, x =>
                {
                    path = PathSelector(x.RootDirectory.FullName);
                }, temp));
            }
            var pMenu = new ConsoleMenu<DriveInfo>("Drives", driveItems);
            pMenu.RunConsoleMenu();

            return path;
        }

    }
}
