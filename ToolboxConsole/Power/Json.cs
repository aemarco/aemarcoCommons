using Spectre.Console.Json;
using System.Text.Json;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ToolboxConsole;

public static partial class PowerConsole
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true
    };
    public static void WriteAsJson(object obj, JsonSerializerOptions? options = null)
    {
        var json = JsonSerializer.Serialize(
            obj,
            options ?? JsonSerializerOptions);
        AnsiConsole.Write(new JsonText(json));
        AnsiConsole.WriteLine();
    }
}
