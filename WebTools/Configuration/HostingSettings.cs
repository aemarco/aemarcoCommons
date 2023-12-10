using aemarcoCommons.ToolboxAppOptions;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace aemarcoCommons.WebTools.Configuration;

public class HostingSettings : SettingsBase
{

    /// <summary>
    /// Settings for JWT Checks
    /// </summary>
    public string? JwtAuthority { get; set; }
    public string? JwtCertPath { get; set; }
    public string? JwtCertPwd { get; set; }

    /// <summary>
    /// Settings for oidc checks
    /// </summary>
    public string? OidcAuthority { get; set; }
    public string? OidcClientId { get; set; }
    public string? OidcClientSecret { get; set; }



    /// <summary>
    /// Enables DeveloperExceptionPage
    /// </summary>
    public bool UseDeveloperExceptionPage { get; set; }
}