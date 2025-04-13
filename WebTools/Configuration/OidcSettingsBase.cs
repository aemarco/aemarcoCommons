// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace aemarcoCommons.WebTools.Configuration;

public class OidcSettingsBase
{
    public required string Authority { get; init; }
    public string? ClientId { get; init; }
    public string? ClientSecret { get; init; }

    /// <summary>
    /// Need to be allowed in the identity provider
    /// oidc scopes:
    /// - openid already included
    /// - profile already included
    ///
    /// common scopes:
    /// - email
    /// - roles
    /// - offline_access
    /// </summary>
    public required string[] Scopes { get; init; }


    /// <summary>
    /// Need to be allowed in the identity provider
    /// </summary>
    public string CallbackPath { get; init; } = "/signin-oidc";
    /// <summary>
    /// Need to be allowed in the identity provider
    /// </summary>
    public string SignedOutCallbackPath { get; init; } = "/signout-callback-oidc";
    /// <summary>
    /// After successful signout, we navigate to this URL.
    /// </summary>
    public string SignedOutRedirectUri { get; init; } = "/";


}