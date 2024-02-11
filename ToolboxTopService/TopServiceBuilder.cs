namespace aemarcoCommons.ToolboxTopService;

public partial class TopServiceBuilder
{

    private readonly string _dllPath;
    public TopServiceBuilder(string dllPath)
    {
        _dllPath = dllPath;
    }

    private string _serviceName = "DefaultServiceName";
    public TopServiceBuilder ServiceName(string serviceName)
    {
        _serviceName = serviceName;
        return this;
    }

    private string _displayName = "DefaultDisplayName";
    public TopServiceBuilder DisplayName(string displayName)
    {
        _displayName = displayName;
        return this;
    }

    private string _description = string.Empty;
    public TopServiceBuilder Description(string description)
    {
        _description = description;
        return this;
    }

    private StartupType _startupType;
    public TopServiceBuilder StartupType(StartupType startupType)
    {
        _startupType = startupType;
        return this;
    }

    internal TopService Build()
    {
        var result = new TopService(
            _dllPath,
            _serviceName,
            _displayName,
            _description,
            _startupType);
        return result;
    }
}

public enum StartupType
{
    Manual,
    Auto,
    AutoDelayed,
    Disabled
}

