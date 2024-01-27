namespace aemarcoCommons.ToolboxTopService;

public class TopService
{
    internal TopService(string dllPath)
    {
        DllPath = dllPath;
    }
    internal string DllPath { get; }


    public string ServiceName { get; set; } = "DefaultServiceName";
    public string DisplayName { get; set; } = "DefaultDisplayName";
    public string Description { get; set; } = string.Empty;


}

public static class TopServiceExtensions
{
    public static TopService SetServiceName(this TopService service, string serviceName)
    {
        service.ServiceName = serviceName;
        return service;
    }

    public static TopService SetDisplayName(this TopService service, string displayName)
    {
        service.DisplayName = displayName;
        return service;
    }

    public static TopService SetDescription(this TopService service, string description)
    {
        service.Description = description;
        return service;
    }
}
