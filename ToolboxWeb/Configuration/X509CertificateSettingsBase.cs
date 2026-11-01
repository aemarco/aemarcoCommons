using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace aemarcoCommons.ToolboxWeb.Configuration;

/// <summary>
/// If you are using this (unchanged), you can call 'ApplyTo' on it, to apply Authority, Audience and signing key
/// straight onto a <see cref="TokenValidationParameters"/> instance.
/// </summary>
/// <remarks>
/// If you extend, you need to roll your own mapper.
/// To use together with ToolboxAppOptions, just do
/// <code>public class CertSettings : X509CertificateSettingsBase, ISettingsBase;</code>
/// </remarks>
public class X509CertificateSettingsBase
{
    public required string Authority { get; init; }
    public required string Audience { get; init; }
    public required string Path { get; init; }
    public required string Pwd { get; init; }
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Source)]
public static partial class CertSettingsBaseMapping
{
    [MapPropertyFromSource(nameof(TokenValidationParameters.IssuerSigningKey), Use = nameof(GetKey))]
    [MapProperty(nameof(X509CertificateSettingsBase.Authority), nameof(TokenValidationParameters.ValidIssuer))]
    [MapProperty(nameof(X509CertificateSettingsBase.Audience), nameof(TokenValidationParameters.ValidAudience))]
    public static partial void ApplyTo(this X509CertificateSettingsBase settings, TokenValidationParameters options);
    private static SecurityKey GetKey(X509CertificateSettingsBase settings)
    {
        var cert = X509CertificateLoader.LoadPkcs12FromFile(settings.Path, settings.Pwd);
        return new X509SecurityKey(cert);
    }
}