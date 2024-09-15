namespace aemarcoCommons.ToolboxTopService;

public class TopServiceBuilder
{

    private readonly string _dllPath;
    internal TopServiceBuilder(HostApplicationBuilder app)
    {
        _serviceName = app.Environment.ApplicationName;
        _displayName = app.Environment.ApplicationName;

        var args = Environment.GetCommandLineArgs();
        _dllPath = args[0];
    }

    private string _serviceName;
    public TopServiceBuilder ServiceName(string serviceName)
    {
        _serviceName = serviceName;
        return this;
    }

    private string _displayName;
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

