namespace aemarcoCommons.Toolbox.Oidc
{
    public interface IOidcSettings
    {
        int LocalPort { get; }

        string Authority { get; }

        string ClientId { get; }

        string Scope { get; }

        string PostLoginUrl { get; }

    }
}