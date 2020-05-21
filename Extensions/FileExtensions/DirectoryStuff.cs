using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Extensions.FileExtensions
{
    public static class DirectoryStuff
    {
        public static void TryDeleteEmptySubfolders(this DirectoryInfo dir)
        {
            TryDeleteEmptySubfolders(dir.FullName);
        }

        //so that is home is not visible to others
        private static readonly string[] _ignoredFiles = 
        {
            "desktop.ini",
            "thumbs.db",
        };

        private static readonly string[] _ignoredExtensions = 
        {
            ".tmp"
        };
        
        private static void TryDeleteEmptySubfolders(string path, bool isHome = true)
        {
            try
            {
                //for subfolder
                foreach (var strSubDir in Directory.GetDirectories(path))
                {
                    //recursive for each subfolder
                    if (!strSubDir.Contains("System Volume Information") &&
                        !strSubDir.Contains("$RECYCLE.BIN") &&
                        !strSubDir.Contains("#recycle"))
                        TryDeleteEmptySubfolders(strSubDir, false);
                }
                // never delete home
                if (isHome) return;
                
                
                // delete from bottom up
                if (!new DirectoryInfo(path).GetFiles("*", SearchOption.AllDirectories).Any(x =>
                    !_ignoredFiles.Contains(x.Name.ToLower()) ||
                    !_ignoredExtensions.Contains(x.Extension.ToLower())))
                {
                    Directory.Delete(path, true);
                }
            }
            catch
            {
                // ignored
            }
        }

    }
}
