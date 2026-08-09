using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace aemarcoCommons.ToolboxWeb.Authentication;

public static class AuthenticationExtensions
{

    /// <summary>
    /// Registers a scheme that authenticates every request as an anonymous identity, performing no real check.
    /// Useful for policies where identity itself isn't the gate (e.g. LAN-IP-based ones), since ASP.NET Core's
    /// pipeline always needs some scheme to produce a ClaimsPrincipal before a policy's requirements can run.
    /// Never register it as the default scheme — that would let every request through as "authenticated".
    /// </summary>
    public static AuthenticationBuilder AddAnonymousScheme(this AuthenticationBuilder builder, string scheme)
    {
        builder.AddScheme<AuthenticationSchemeOptions, AlwaysSucceedAuthenticationHandler>(scheme, _ => { });
        return builder;
    }
}

public class AlwaysSucceedAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public AlwaysSucceedAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var ticket = new AuthenticationTicket(
            new ClaimsPrincipal(
                new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.Name, "Anonymous")
                    ],
                    "AnonymousAuthType")),
            Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

}
