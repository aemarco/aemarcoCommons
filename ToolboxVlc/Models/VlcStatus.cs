// ReSharper disable NotAccessedPositionalProperty.Global
namespace aemarcoCommons.ToolboxVlc.Models;

public record VlcStatus(
    int Time,
    int Volume,
    int Length,
    string State);
