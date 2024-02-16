using System;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.Toolbox.Oidc
{
    [Obsolete("Use SystemBrowser instead")]
    public class OidcSystemBrowser : SystemBrowser
    {
        public OidcSystemBrowser(int? port, string postLoginUrl = null)
            : base(port, postLoginUrl)
        {
        }

    }
}
