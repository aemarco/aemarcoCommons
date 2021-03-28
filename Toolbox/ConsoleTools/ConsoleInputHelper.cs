using aemarcoCommons.Toolbox.NetworkTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aemarcoCommons.Toolbox.ConsoleTools
{
    public static class ConsoleInputHelper
    {

        #region bool

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

        #endregion

        #region string

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

        #endregion

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

        #region class based selection

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


        //selection logic
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

        #endregion

        #region folder, drive and share navigation

        /// <summary>
        /// Selection of a path in a ConsoleMenu
        /// </summary>
        /// <param name="path">path to start at</param>
        /// <param name="includedServers">servers to include for share selection</param>
        /// <param name="showHidden">shot hidden folders</param>
        /// <returns>selected path</returns>
        public static string PathSelector(string path, IEnumerable<string> includedServers = null, bool showHidden = false)
        {
            var serverItems = includedServers?.ToArray();
            var dir = new DirectoryInfo(path);
            if (!dir.Exists) throw new DirectoryNotFoundException($"{path}");
            Console.Clear();

            //one level up
            var dirItems = new List<ConsoleMenuItem>
            {
                new ConsoleMenuItem<DirectoryInfo>("..", x =>
                {
                    path = PathSelector(
                        x is null
                            ? DriveSelector(serverItems) :
                            x.FullName,
                        serverItems,
                        showHidden);
                }, dir.Parent)
            };

            //one level down
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var sub in dir
                .GetDirectories()
                .Where(x => showHidden || !x.Attributes.HasFlag(FileAttributes.Hidden)
                ))
            {
                var temp = sub;
                dirItems.Add(new ConsoleMenuItem<DirectoryInfo>(temp.Name, x =>
                {
                    path = PathSelector(x.FullName, serverItems, showHidden);
                }, temp));
            }

            //select
            dirItems.Add(new ConsoleMenuSeparator());
            dirItems.Add(new ConsoleMenuItem<DirectoryInfo>("Select", x =>
            {
                path = x.FullName;

            }, dir));

            var menu = new ConsoleMenu(dir.FullName, dirItems);
            menu.RunConsoleMenu();

            return path;
        }




        /// <summary>
        /// Selection of a drive in a ConsoleMenu
        /// </summary>
        /// <returns></returns>
        public static string DriveSelector(IEnumerable<string> servers = null)
        {
            string path = null;
            Console.Clear();

            var driveItems = new List<ConsoleMenuItem>();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var drive in DriveInfo.GetDrives()
                .Where(x => x.IsReady))
            {
                var temp = drive;
                driveItems.Add(new ConsoleMenuItem<DriveInfo>($"{temp.Name} ({temp.VolumeLabel})", x =>
                {
                    path = x.RootDirectory.FullName;
                }, temp));
            }

            if (servers != null && servers.ToList() is var list && list.Count > 0)
            {
                driveItems.Add(new ConsoleMenuSeparator());
                driveItems.Add(new ConsoleMenuItem<DriveInfo>("Network", _ =>
                {
                    path = ShareSelector(list) ?? DriveSelector(list);
                }, null));
            }
            var menu = new ConsoleMenu("Drives", driveItems);
            menu.RunConsoleMenu();

            return path;
        }


        /// <summary>
        /// Selection of a share for given servers
        /// </summary>
        /// <param name="servers">servers which can be selected</param>
        /// <returns></returns>
        public static string ShareSelector(IEnumerable<string> servers)
        {
            var serverItems = servers.ToList();
            switch (serverItems.Count)
            {
                case 0:
                    return null; //shortcut for no server
                case 1:
                    return ShareSelector(serverItems[0]); //shortcut for one server
            }

            //rely on a selection of which server to use
            var serverItem = AbortableSelection("Server", serverItems, x => x);
            var path = ShareSelector(serverItem);
            return path;
        }

        /// <summary>
        /// Selection of a share for given server
        /// </summary>
        /// <returns>unc path to share or null when no shares are available</returns>
        public static string ShareSelector(string server)
        {
            Console.Clear();
            string path = null;
            var shares = new List<ConsoleMenuItem<DirectoryInfo>>();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (Share share in ShareCollection.GetShares(server))
            {
                if (!share.IsFileSystem || share.ShareType != ShareType.Disk) continue;

                var temp = share.Root;
                shares.Add(new ConsoleMenuItem<DirectoryInfo>($"{share.Server} --> {share.NetName}", x =>
                {
                    path = x.FullName;
                }, temp));
            }
            if (shares.Count == 0) return path;

            var menu = new ConsoleMenu($"{server} shares", shares);
            menu.RunConsoleMenu();
            return path;
        }

        #endregion

    }
}
