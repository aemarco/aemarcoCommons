using System.IO;

namespace aemarcoCommons.Extensions.StorageExtensions;

public static class FileOpsExtensions
{
    public static void MoveToFolder(this FileInfo file, string targetPath, bool overwrite = false)
    {
        file.CopyToFolder(targetPath, overwrite);
        File.Delete(file.FullName);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static void CopyToFolder(this FileInfo file, string targetPath, bool overwrite = false)
    {
        var target = Path.Combine(targetPath, file.Name);
        File.Copy(file.FullName, target, overwrite);
    }
}