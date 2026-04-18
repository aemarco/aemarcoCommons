using Newtonsoft.Json;
using Spectre.Console.Json;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools;

public static partial class PowerConsole
{
    [Obsolete("Use aemarcoCommons.ToolboxConsole.PowerConsole instead.")]
    public static void WriteAsJson(object o)
    {
        AnsiConsole.Write(new JsonText(JsonConvert.SerializeObject(o)));
        AnsiConsole.WriteLine();
    }

}
