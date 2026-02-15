namespace aemarcoCommons.ToolboxTypeStore.Models;

public class TypeToFileSettings
{
    public string? StorageDirectory { get; set; }
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new();
}
