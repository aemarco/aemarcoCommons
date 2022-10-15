using aemarcoCommons.Toolbox.AppConfiguration;

// ReSharper disable UnusedAutoPropertyAccessor.Global

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
    public class OidcSettings : SettingsBase, IOidcSettings
    {
        public virtual int LocalPort { get; set; }
        public virtual string Authority { get; set; }
        public virtual string ClientId { get; set; }
        public virtual string Scope { get; set; }
        public virtual string PostLoginUrl { get; set; }
    }
}
