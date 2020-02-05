using System;
using System.IO;
using Newtonsoft.Json;

namespace Extensions.netExtensions
{
    public static class VersionExtensions
    {
        public static void ToTextFile(this Version version, string filePath)
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(version, Formatting.Indented));
        }

        public static Version ToVersionFromTextFile(this FileInfo file, Version defaultResult = null)
        {
            return (file.Exists)
                ? JsonConvert.DeserializeObject<Version>(File.ReadAllText(file.FullName))
                : defaultResult;
        }
    }
}
