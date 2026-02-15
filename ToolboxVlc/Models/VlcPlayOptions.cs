namespace aemarcoCommons.ToolboxVlc.Models;

public class VlcPlayOptions
{
    public string? Title { get; set; }
    public string? Url { get; set; }
    public string? Genre { get; set; }
    public bool Fullscreen { get; set; }
    public bool PlayAndExit { get; set; } = true;

}

public class VlcPlayOptionsBuilder
{
    private readonly VlcPlayOptions _options = new();

    public VlcPlayOptionsBuilder WithTitle(string? title)
    {
        _options.Title = title;
        return this;
    }
    public VlcPlayOptionsBuilder WithUrl(string? url)
    {
        _options.Url = url;
        return this;
    }
    public VlcPlayOptionsBuilder WithGenre(string? genre)
    {
        _options.Genre = genre;
        return this;
    }
    public VlcPlayOptionsBuilder Fullscreen(bool fullscreen = false)
    {
        _options.Fullscreen = fullscreen;
        return this;
    }
    public VlcPlayOptionsBuilder PlayAndExit(bool playAndExist = true)
    {
        _options.PlayAndExit = playAndExist;
        return this;
    }
    public VlcPlayOptions Build()
    {
        return _options;
    }
}