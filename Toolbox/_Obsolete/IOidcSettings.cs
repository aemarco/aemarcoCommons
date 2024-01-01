

using System;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.Toolbox.Oidc
{
    [Obsolete]
    public interface IOidcSettings
    {
        int LocalPort { get; }

        string Authority { get; }

        string ClientId { get; }

        string Scope { get; }

        string PostLoginUrl { get; }

    }
}