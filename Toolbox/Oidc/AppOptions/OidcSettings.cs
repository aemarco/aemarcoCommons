

// ReSharper disable UnusedAutoPropertyAccessor.Global

using aemarcoCommons.ToolboxAppOptions;

namespace aemarcoCommons.Toolbox.Oidc.AppOptions
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
