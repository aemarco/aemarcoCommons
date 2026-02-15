namespace aemarcoCommons.ToolboxVlc.Contracts;

public interface IVlcClient
{
    public string HttpPassword { get; }
    Task<VlcStatus?> GetStatus(CancellationToken cancellationToken = default);
}
