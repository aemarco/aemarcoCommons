using System;
using System.Collections.Generic;
using System.IO;

namespace Toolbox.ConsoleTools
{
    public static class ConsoleMenuHelper
    {
        public static string RunPathSelector(string path)
        {
            Console.Clear();
            var dir = new DirectoryInfo(path);
            var dirItems = new List<ConsoleMenuItem>
            {
                new ConsoleMenuItem<DirectoryInfo>("..", x =>
                {
                    if (x != null)
                        path = RunPathSelector(x.FullName);
                    else
                        path = RunDriveSelector();
                }, dir.Parent)
            };

            foreach (var sub in dir.GetDirectories())
            {
                if (sub.Attributes.HasFlag(FileAttributes.Hidden)) continue;

                var temp = sub;
                dirItems.Add(new ConsoleMenuItem<DirectoryInfo>(temp.Name, x =>
                {
                    path = RunPathSelector(temp.FullName);

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

        static string RunDriveSelector()
        {
            string path = null;
            Console.Clear();

            var driveItems = new List<ConsoleMenuItem<DriveInfo>>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (!drive.IsReady) continue;

                var temp = drive;
                driveItems.Add(new ConsoleMenuItem<DriveInfo>(temp.Name, x =>
                {
                    path = RunPathSelector(x.RootDirectory.FullName);
                }, temp));
            }
            var pMenu = new ConsoleMenu<DriveInfo>("Drives", driveItems);
            pMenu.RunConsoleMenu();

            return path;
        }

    }
}
