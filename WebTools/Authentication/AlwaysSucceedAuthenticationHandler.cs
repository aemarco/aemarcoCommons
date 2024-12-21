using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace aemarcoCommons.WebTools.Authentication;

public static class AuthenticationExtensions
{

    /// <summary>
    /// Bypasses the Authentication... Ensure it is not used as default scheme
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="scheme"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddAnonymousScheme(this AuthenticationBuilder builder, string scheme)
    {
        builder
            .AddScheme<AuthenticationSchemeOptions, AlwaysSucceedAuthenticationHandler>(scheme, _ =>
            {

            });
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
