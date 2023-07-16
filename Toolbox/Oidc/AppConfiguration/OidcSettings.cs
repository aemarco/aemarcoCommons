using aemarcoCommons.Toolbox.AppConfiguration;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace aemarcoCommons.Toolbox.Oidc.AppConfiguration
{
    public class OidcSettings : SettingsBase, IOidcSettings
    {
        public virtual int LocalPort { get; set; }
        public virtual string Authority { get; set; }
        public virtual string ClientId { get; set; }
        public virtual string Scope { get; set; }
        public virtual string PostLoginUrl { get; set; }
    }
}
