using System;
using System.IO;
using Newtonsoft.Json;

namespace Extensions.netExtensions
{
    public static class VersionExtensions
    {
        public static void ToTextFile(this Version version, string filePath)
        {
            File.WriteAllText(filePath, version.ToString());
        }


        public static Version ToVersionFromTextFile(this FileInfo file, string defaultVersion = "1.0.0.0")
        {
            return (file.Exists)
                ? Version.Parse(File.ReadAllText(file.FullName))
                : Version.Parse(defaultVersion);
        }
    }
}
