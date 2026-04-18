using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Riok.Mapperly.Abstractions;

namespace aemarcoCommons.WebTools.Oidc;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Source)]
public static partial class OidcSettingsBaseMapping
{
    [MapProperty(nameof(OidcSettingsBase.Scopes), nameof(OpenIdConnectOptions.Scope))]
    public static partial void ApplyTo(this OidcSettingsBase settings, OpenIdConnectOptions options);

}
