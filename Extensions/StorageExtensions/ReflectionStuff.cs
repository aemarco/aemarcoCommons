using System.IO;
using System.Reflection;

namespace aemarcoCommons.Extensions.StorageExtensions;

public static class ReflectionStuff
{

    public static string GetEntryDirectory()
    {
        return Assembly.GetEntryAssembly().GetDirectory();
    }


    public static string GetDirectory(this Assembly assembly)
    {
        var assemblyLocation = assembly.Location;
        var di = new FileInfo(assemblyLocation).Directory;
        return di?.FullName;
    }

}