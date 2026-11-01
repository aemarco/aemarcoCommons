using System.IO;

namespace aemarcoCommons.Extensions.NetworkExtensions;

public record NetworkPathAccessibilityResult(
    bool IsAccessible,
    Exception? Exception = null);

public static class PathExtensions
{
    public static NetworkPathAccessibilityResult CheckNetworkPathAccessibility(this DirectoryInfo directory)
    {
        try
        {
            var networkRoot = Path.GetPathRoot(directory.FullName);
            var isAccessible = !string.IsNullOrEmpty(networkRoot) && Directory.Exists(networkRoot);
            return new NetworkPathAccessibilityResult(isAccessible);
        }
        catch (IOException ex)
        {
            return new NetworkPathAccessibilityResult(false, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            return new NetworkPathAccessibilityResult(false, ex);
        }
    }
}