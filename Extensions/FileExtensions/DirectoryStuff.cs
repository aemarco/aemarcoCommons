using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace aemarcoCommons.Extensions.FileExtensions;

public static class DirectoryStuff
{
    public static void TryDeleteEmptySubfolders(this DirectoryInfo dir)
    {
        TryDeleteEmptySubfolders(dir.FullName);
    }

    //so that is home is not visible to others
    private static readonly string[] IgnoredFiles =
    {
        "desktop.ini",
        "thumbs.db",
    };

    private static readonly string[] IgnoredExtensions =
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
                    !IgnoredFiles.Contains(x.Name.ToLower()) ||
                    !IgnoredExtensions.Contains(x.Extension.ToLower())))
            {
                Directory.Delete(path, true);
            }
        }
        catch
        {
            // ignored
        }
    }









    /// <summary>
    /// Ensures that <paramref name="targetFolder"/> contains exactly the given files by *file name*.
    /// 
    /// - Existing files with matching names are kept as-is.
    /// - Files not listed are deleted.
    /// - Files listed but missing are copied from their source paths.
    /// - Duplicate file names ?? First file wins, others are ignored
    /// 
    /// Note: Matching is done by file name only (not content or path).
    /// </summary>
    /// <param name="targetFolder">absolute target folder path</param>
    /// <param name="sourceFiles">absolute file paths for source files</param>
    /// <param name="cancellationToken">cancellation</param>
    public static void SyncFolderByFileName(this DirectoryInfo targetFolder, IEnumerable<string> sourceFiles, CancellationToken cancellationToken = default)
    {
        targetFolder.Create();
        var desiredFiles = sourceFiles
            .Select(x => new FileInfo(x))
            .ToDictionary(
                x => x.FullName,
                x => Path.Combine(targetFolder.FullName, x.Name));

        var existingFiles = targetFolder
            .GetFiles("*.*", SearchOption.AllDirectories)
            .ToList();

        //delete obsolete files
        foreach (var file in existingFiles
                     .Where(x => !desiredFiles.ContainsValue(x.FullName)))
        {
            cancellationToken.ThrowIfCancellationRequested();
            file.TryDelete();
        }


        //copy new files
        foreach (var kvp in desiredFiles)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (File.Exists(kvp.Value))
                continue;

            File.Copy(kvp.Key, kvp.Value);
        }
    }






}