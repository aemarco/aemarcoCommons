using Spectre.Console.Json;
using System.Text.Json;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ToolboxConsole;

public static partial class PowerConsole
{
    public static void WriteAsJson(object obj, JsonSerializerOptions? options = null)
    {
        var json = JsonSerializer.Serialize(obj, options);
        AnsiConsole.Write(new JsonText(json));
        AnsiConsole.WriteLine();
    }
}
