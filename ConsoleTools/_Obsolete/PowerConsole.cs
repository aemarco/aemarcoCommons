using System.Collections.Generic;
using System.Runtime.Versioning;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools;
public static partial class PowerConsole
{
    /// <summary>
    /// Selection of a share for given servers
    /// </summary>
    /// <param name="servers">servers which can be selected</param>
    /// <returns></returns>
    [SupportedOSPlatform("windows")]
    [Obsolete("Use ServerSelector instead.")]
    public static string ShareSelector(IEnumerable<string> servers) => ServerSelector(servers);



}
