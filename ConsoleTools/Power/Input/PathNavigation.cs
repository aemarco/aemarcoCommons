using aemarcoCommons.Toolbox.NetworkTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;


// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools
{
    public static partial class PowerConsole
    {

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

            if (OperatingSystem.IsWindows() && servers != null && servers.ToList() is var list && list.Count > 0)
            {
                driveItems.Add(new ConsoleMenuSeparator());
                driveItems.Add(new ConsoleMenuItem<DriveInfo>("Network", _ =>
                {
                    if (!OperatingSystem.IsWindows()) throw new NotImplementedException();
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
        [SupportedOSPlatform("windows")]
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
        [SupportedOSPlatform("windows")]
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
    }
}