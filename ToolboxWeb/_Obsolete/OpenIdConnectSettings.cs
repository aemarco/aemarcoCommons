using Microsoft.AspNetCore.Authentication.OpenIdConnect;
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable IDE0130

namespace aemarcoCommons.ToolboxWeb.OpenIdConnect;

/// <summary>
/// If you are using this (unchanged), you can call 'ApplyTo' on it, to directly apply it to OpenIdConnectOptions.
/// </summary>
/// <remarks>If you extend, you need to roll your own mapper</remarks>
[Obsolete("Use aemarcoCommons.ToolboxWeb.Configuration.OpenIdConnectSettings")]
public class OpenIdConnectSettings
{

    /// <summary>
    /// Gets or sets the Authority to use when making OpenIdConnect calls.
    /// </summary>
    public required string Authority { get; init; }
    /// <summary>
    /// Gets or sets the 'client_id'.
    /// </summary>
    public string? ClientId { get; init; }
    /// <summary>
    /// Gets or sets the 'client_secret'.
    /// </summary>
    public string? ClientSecret { get; init; }

    /// <summary>
    /// Gets the list of permissions to request.
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
    /// <remarks>Need to be allowed in the identity provider</remarks>
    public required string[] Scopes { get; init; }

    /// <summary>
    /// The request path within the application's base path where the user-agent will be returned.
    /// The middleware will process this request when it arrives.
    /// </summary>
    /// <remarks>Need to be allowed in the identity provider as 'RedirectUris'</remarks>
    public string CallbackPath { get; init; } = "/signin-oidc";
    /// <summary>
    /// The request path within the application's base path where the user agent will be returned after sign out from the identity provider.
    /// See post_logout_redirect_uri from http://openid.net/specs/openid-connect-session-1_0.html#RedirectionAfterLogout.
    /// </summary>
    /// <remarks>Need to be allowed in the identity provider as 'PostLogoutRedirectUris'</remarks>
    public string SignedOutCallbackPath { get; init; } = "/signout-callback-oidc";
    /// <summary>
    /// The uri where the user agent will be redirected to after application is signed out from the identity provider.
    /// The redirect will happen after the SignedOutCallbackPath is invoked.
    /// </summary>
    /// <remarks>This URI can be out of the application's domain. By default it points to the root.</remarks>
    public string SignedOutRedirectUri { get; init; } = "/";

}

public static class OpenIdConnectSettingsMapping
{
    public static void ApplyTo(this OpenIdConnectSettings settings, OpenIdConnectOptions options)
    {
        options.Authority = settings.Authority;
        options.ClientId = settings.ClientId;
        options.ClientSecret = settings.ClientSecret;
        options.SignedOutCallbackPath = (PathString)settings.SignedOutCallbackPath;
        options.SignedOutRedirectUri = settings.SignedOutRedirectUri;
        foreach (var item in settings.Scopes)
        {
            options.Scope.Add(item);
        }
        options.CallbackPath = (PathString)settings.CallbackPath;
    }
}