// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ToolboxTopService;

public partial class TopServiceBuilder
{
    [Obsolete("Use ServiceName instead.")]
    public TopServiceBuilder SetServiceName(string serviceName) => ServiceName(serviceName);

    [Obsolete("Use DisplayName instead.")]
    public TopServiceBuilder SetDisplayName(string displayName) => DisplayName(displayName);

    [Obsolete("Use Description instead.")]
    public TopServiceBuilder SetDescription(string description) => Description(description);

}