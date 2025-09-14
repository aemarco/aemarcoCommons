using IdentityModel.OidcClient;
using Microsoft.Extensions.DependencyInjection;
using System;
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
        OidcTokenRenewalHandlerHelper oidcTokenRenewalHandlerHelper,
        IServiceProvider serviceProvider)
    {
        _oidcTokenRenewalHandlerHelper = oidcTokenRenewalHandlerHelper;
        _serviceProvider = serviceProvider;
    }


    private OidcClient _oidcClient;
    private ISessionStore _sessionStore;
    private void EnsureSetup()
    {
        if (_oidcClient == null)
        {
            _oidcClient = _serviceProvider.GetRequiredService<OidcClient>();
        }
        if (_sessionStore == null)
        {
            _sessionStore = _serviceProvider.GetRequiredService<ISessionStore>();
        }
    }


    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        EnsureSetup();

        //if we have no access token, we try to get one with refresh
        var session = await GetAccessTokenAsync(cancellationToken)
            .ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(session?.AccessToken))
        {
            if (await RefreshTokensAsync(session, cancellationToken)
                    .ConfigureAwait(false))
            {
                //seems we will get a new session
                session = await _sessionStore.GetSession();
                if (string.IsNullOrWhiteSpace(session?.AccessToken))
                {
                    //otherwise we fail
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized) { RequestMessage = request };
                }
            }
            else
            {
                //otherwise we fail
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
        if (!await RefreshTokensAsync(session, cancellationToken).ConfigureAwait(false))
        {
            //we cant refresh
            return response;
        }

        //seems we refreshed the token
        session = await _sessionStore.GetSession()
            .ConfigureAwait(false);

        response.Dispose(); // This 401 response will not be used for anything so is disposed to unblock the socket.

        //we refreshed the token, so we try the same request once more
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        return await base.SendAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }




    private async Task<Session> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        //in case somebody is refreshing currently, we don´t want to return until refreshed, so locking
        var res = await _oidcTokenRenewalHandlerHelper.HandleLockedAsync(
                () => Task.CompletedTask,
                cancellationToken)
            .ConfigureAwait(false);

        return res
            ? await _sessionStore.GetSession()
            : null;
    }

    private async Task<bool> RefreshTokensAsync(Session session, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(session?.RefreshToken))
        {
            return false;
        }

        return await _oidcTokenRenewalHandlerHelper.HandleLockedAsync(async () =>
            {
                var response = await _oidcClient.RefreshTokenAsync(session.RefreshToken, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                if (response.IsError)
                    throw new Exception("Could not refresh token");

                await _sessionStore.SetSession(
                        new Session
                        {
                            IdToken = response.IdentityToken,
                            AccessToken = response.AccessToken,
                            RefreshToken = response.RefreshToken
                        })
                    .ConfigureAwait(false);

            }, cancellationToken)
            .ConfigureAwait(false);

    }


}

// ReSharper disable once ClassNeverInstantiated.Global
public class OidcTokenRenewalHandlerHelper
{
    private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
    private TimeSpan LockTimeout { get; } = TimeSpan.FromSeconds(5);


    public async Task<bool> HandleLockedAsync(Func<Task> action, CancellationToken cancellationToken)
    {
        if (await _lock.WaitAsync(LockTimeout, cancellationToken)
                .ConfigureAwait(false))
        {
            try
            {
                await action()
                    .ConfigureAwait(false);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _lock.Release();
            }
        }
        return false;
    }

}