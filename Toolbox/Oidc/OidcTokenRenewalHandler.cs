using IdentityModel;
using IdentityModel.OidcClient;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.Toolbox.Oidc;

/// <summary>
/// this handler can be chained in a HttpClient, so that we try to gather a new access token when we encounter 401 responses.
/// if this handler returns a 401, means that the access token + refresh token are no more usable
/// </summary>
public class OidcTokenRenewalHandler : DelegatingHandler
{


    private readonly IServiceProvider _serviceProvider;
    private readonly OidcTokenRenewalHandlerHelper _oidcTokenRenewalHandlerHelper;
    public OidcTokenRenewalHandler(
        IServiceProvider serviceProvider,
        OidcTokenRenewalHandlerHelper oidcTokenRenewalHandlerHelper)
    {
        _serviceProvider = serviceProvider;
        _oidcTokenRenewalHandlerHelper = oidcTokenRenewalHandlerHelper;

    }

    private ISessionStore? _sessionStore;
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _sessionStore ??= _serviceProvider.GetRequiredService<ISessionStore>();
        var session = await _sessionStore.GetSession()
            .ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(session?.AccessToken))
        {//if we have no access token, we try to get one with refresh

            if (await _oidcTokenRenewalHandlerHelper.RefreshTokensAsync(cancellationToken)
                    .ConfigureAwait(false))
            {
                //seems we will get a new session
                session = await _sessionStore.GetSession()
                    .ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(session?.AccessToken))
                {
                    //otherwise we fail
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized) { RequestMessage = request };
                }
            }
            else
            {
                //otherwise we fail
                await _sessionStore.EndSession()
                    .ConfigureAwait(false);
                return new HttpResponseMessage(HttpStatusCode.Unauthorized) { RequestMessage = request };
            }
        }

        //so seems we have an access token, so try to use it
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        var response = await base.SendAsync(request, cancellationToken)
            .ConfigureAwait(false);


        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            //happy path
            return response;
        }

        //if we get 401, we try to refresh the access token
        if (!await _oidcTokenRenewalHandlerHelper.RefreshTokensAsync(cancellationToken).ConfigureAwait(false))
        {
            //we cant refresh
            await _sessionStore.EndSession()
                .ConfigureAwait(false);
            return response;
        }

        //seems we refreshed the token
        session = await _sessionStore.GetSession()
            .ConfigureAwait(false);

        response.Dispose(); // This 401 response will not be used for anything so is disposed to unblock the socket.

        if (string.IsNullOrWhiteSpace(session?.AccessToken))
            return new HttpResponseMessage(HttpStatusCode.Unauthorized) { RequestMessage = request };

        //we refreshed the token, so we try the same request once more
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        return await base.SendAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }

}

// ReSharper disable once ClassNeverInstantiated.Global
public class OidcTokenRenewalHandlerHelper
{


    private readonly IServiceProvider _serviceProvider;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private TimeSpan LockTimeout { get; } = TimeSpan.FromSeconds(5);
    public OidcTokenRenewalHandlerHelper(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }


    private ISessionStore? _sessionStore;
    private OidcClient? _oidcClient;
    public async Task<bool> RefreshTokensAsync(CancellationToken cancellationToken)
    {

        _sessionStore ??= _serviceProvider.GetRequiredService<ISessionStore>();
        _oidcClient ??= _serviceProvider.GetRequiredService<OidcClient>();

        if (!await _lock.WaitAsync(LockTimeout, cancellationToken)
                .ConfigureAwait(false))
            throw new TimeoutException("Could not acquire refresh lock in time");

        try
        {
            var session = await _sessionStore.GetSession()
                .ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(session?.RefreshToken))
                return false;


            var response = await _oidcClient.RefreshTokenAsync(session.RefreshToken, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (response.IsError)
            {
                if (response.Error == OidcConstants.TokenErrors.InvalidGrant)
                    return false;

                throw new Exception($"Could not refresh token with Error {response.Error} and description {response.ErrorDescription}");
            }

            await _sessionStore.SetSession(
                    new Session
                    {
                        IdToken = response.IdentityToken,
                        AccessToken = response.AccessToken,
                        RefreshToken = response.RefreshToken
                    })
                .ConfigureAwait(false);
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }

}