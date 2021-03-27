using System.Diagnostics;
using System.IO;

namespace aemarcoCommons.Extensions.FileExtensions
{
    public static class FileSystemStuff
    {
        public static Process OpenFileOrFolder(this FileInfo fileOrFolderPath)
        {
            var result = new Process
            {
                StartInfo = new ProcessStartInfo(fileOrFolderPath.FullName)
                {
                    UseShellExecute = true
                }
            };
            result.Start();
            return result;
        }
    }
}
