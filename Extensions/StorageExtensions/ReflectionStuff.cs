using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;


namespace aemarcoCommons.Extensions.StorageExtensions;

public static class ReflectionStuff
{

    public static string? GetEntryDirectory()
    {
        return Assembly.GetEntryAssembly()?.GetDirectory();
    }


    [return: NotNullIfNotNull(nameof(assembly))]
    public static string? GetDirectory(this Assembly? assembly)
    {
        var assemblyLocation = assembly?.Location;
        if (string.IsNullOrEmpty(assemblyLocation)) return null;

        var di = new FileInfo(assemblyLocation).Directory;
        return di?.FullName;
    }

}