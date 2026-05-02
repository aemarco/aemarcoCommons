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
    /// Synchronizes <paramref name="targetFolder"/> to contain exactly the files specified by <paramref name="sourceFiles"/>, matched by file name only.
    ///
    /// **Operation:**
    /// - Files in target matching a source file name are kept as-is (never overwritten).
    /// - Files in target with no matching source file name are deleted.
    /// - Source files not yet present in target are copied to the target folder root.
    ///
    /// **Limitations:**
    /// - Matching is by file name only (case-sensitive on Linux, case-insensitive on Windows).
    /// - If multiple source files have the same name, the first one is used; others are ignored.
    /// - Deletions that fail due to file locks are logged but do not stop the operation (via TryDelete).
    /// </summary>
    /// <param name="targetFolder">Target folder to synchronize (will be created if missing)</param>
    /// <param name="sourceFiles">Absolute file paths of files that should exist in target folder</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public static void SyncFolderByFileName(this DirectoryInfo targetFolder, IEnumerable<string> sourceFiles, CancellationToken cancellationToken = default)
    {
        targetFolder.Create();
        var desiredFiles = sourceFiles
            .Select(x => new FileInfo(x))
            .ToDictionary(
                x => x.FullName,
                x => Path.Combine(targetFolder.FullName, x.Name));

        var existingFiles = targetFolder
            .GetFiles("*", SearchOption.TopDirectoryOnly)
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