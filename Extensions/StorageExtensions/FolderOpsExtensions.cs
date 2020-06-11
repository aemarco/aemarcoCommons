using System;
using System.IO;
using System.Linq;
using Extensions.netExtensions;

namespace Extensions.StorageExtensions
{
    public static class FolderOpsExtensions
    {

        public static void MoveToFolder(this DirectoryInfo dir, string targetPath, bool overwrite = false, Action<long,long> progress = null)
        {
            long done = 0;
            var total = dir.GetFiles("*", SearchOption.AllDirectories).Sum(x => x.Length);
            
            DoOperation(false, dir, targetPath, overwrite, progress, ref done, ref total);
            dir.Delete(true);
        }


        public static void CopyToFolder(this DirectoryInfo dir, string targetPath, bool overwrite = false, Action<long,long> progress = null)
        {
            long done = 0;
            var total = dir.GetFiles("*", SearchOption.AllDirectories).Sum(x => x.Length);
            DoOperation(true, dir, targetPath, overwrite, progress, ref done, ref total);
        }

        public static void DeleteAllContent(this DirectoryInfo dir)
        {
            dir.DeleteMatchingContent(x => true);
        }

        public static void DeleteMatchingContent(this DirectoryInfo dir, Predicate<FileInfo> filter)
        {
            foreach (var file in dir.GetFiles("*", SearchOption.AllDirectories)
                .Where(x => filter(x))
                .ToList())
            {
                file.TryDelete();
            }
            dir.DeleteEmptySubfolders();
        }


        public static void DeleteEmptySubfolders(this DirectoryInfo dir, bool isHome = true)
        {
            try
            {
                foreach (var subDir in dir.GetDirectories()
                    .ToList()
                    .Where(subDir => 
                        !subDir.FullName.Contains("System Volume Information") &&
                        !subDir.FullName.Contains("$RECYCLE.BIN") &&
                        !subDir.FullName.Contains("#recycle")))
                {
                    //recursion
                    subDir.DeleteEmptySubfolders(false);
                }

                // never delete home
                if (isHome) return;

                //delete from bottom up
                if (!dir.GetFiles("*", SearchOption.AllDirectories).Any()) dir.Delete();
            }
            catch
            {
                // ignored
            }
        }




        // ReSharper disable once SuggestBaseTypeForParameter
        private static void DoOperation(bool copy, DirectoryInfo dir, string targetPath, bool overwrite, Action<long,long> progress, ref long done, ref long total)
        {
            var destDirectory = Path.Combine(targetPath, dir.Name);
            if (!Directory.Exists(destDirectory)) Directory.CreateDirectory(destDirectory);


            var theDirectories = Directory.GetDirectories(dir.FullName).Select(x => new DirectoryInfo(x));
            foreach (var curDir in theDirectories)
            {
                DoOperation(copy, curDir, destDirectory, overwrite, progress, ref done, ref total);
            }

            var theFilesInCurrentDir = Directory.GetFiles(dir.FullName).Select(x => new FileInfo(x));
            foreach(var currentFile in theFilesInCurrentDir)
            {
                var length = currentFile.Length;
                if (copy)
                    currentFile.CopyToFolder(destDirectory, overwrite);
                else
                    currentFile.MoveToFolder(destDirectory, overwrite);

                done += length;
                progress?.Invoke(done, total);
            }
        }
    }
}