namespace aemarcoCommons.ToolboxVlc.Models;

public class VlcOptions
{
    public string HttpPassword { get; set; } = "pass";
}

public class VlcOptionsBuilder
{

    private readonly VlcOptions _options = new();

    public VlcOptionsBuilder WithHttpPassword(string password)
    {
        _options.HttpPassword = password;
        return this;
    }

    public VlcOptions Build()
    {
        return _options;
    }

}
