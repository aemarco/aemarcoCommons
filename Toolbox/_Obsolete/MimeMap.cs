using System.Diagnostics.CodeAnalysis;
using NewMimeMap = aemarcoCommons.Toolbox.FileTools.MimeMap;

#pragma warning disable IDE0130
namespace aemarcoCommons.Toolbox.Mime;
#pragma warning restore IDE0130

[Obsolete("Moved to aemarcoCommons.Toolbox.FileTools.MimeMap. Update your using directive.")]
public static class MimeMap
{
    public static string[] Extensions => NewMimeMap.Extensions;
    public static string[] MimeTypes => NewMimeMap.MimeTypes;

    public static bool TryGetMimeType(string ext, [NotNullWhen(true)] out string? mimeType) =>
        NewMimeMap.TryGetMimeType(ext, out mimeType);

    public static string GetMimeType(string fileNameOrExtension) =>
        NewMimeMap.GetMimeType(fileNameOrExtension);

    public static string? GetExtension(string mimeType) =>
        NewMimeMap.GetExtension(mimeType);
}