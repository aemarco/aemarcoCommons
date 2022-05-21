using System;
using System.IO;

namespace aemarcoCommons.Extensions.VersionExtensions
{
    public static class VersionExtensions
    {

        [Obsolete("Will be removed in future versions")]
        public static void ToTextFile(this Version version, string filePath)
        {
            File.WriteAllText(filePath, version.ToString());
        }


        [Obsolete("Will be removed in future versions")]
        public static Version ToVersionFromTextFile(this FileInfo file, string defaultVersion = "1.0.0.0")
        {
            return (file.Exists)
                ? Version.Parse(File.ReadAllText(file.FullName))
                : Version.Parse(defaultVersion);
        }
    }
}
