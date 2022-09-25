using IdentityModel.OidcClient;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.Toolbox.Oidc
{

    /// <summary>
    /// external party holding the access and refresh token
    /// </summary>
    public interface ISessionStore
    {
        string AccessToken { get; set; }
        string RefreshToken { get; set; }
    }

    /// <summary>
    /// this handler can be chained in a HttpClient, so that we try to gather a new access token when we encounter 401 responses.
    /// if this handler returns a 401, means that the access token + refresh token are no more usable
    /// </summary>
    public class OidcTokenRenewalHandler : DelegatingHandler
    {
        private readonly OidcTokenRenewalHandlerHelper _oidcTokenRenewalHandlerHelper;
        private readonly OidcClient _oidcClient;
        private readonly ISessionStore _sessionStore;

        public OidcTokenRenewalHandler(
            OidcTokenRenewalHandlerHelper oidcTokenRenewalHandlerHelper,
            OidcClient oidcClient,
            ISessionStore sessionStore)
        {
            _oidcTokenRenewalHandlerHelper = oidcTokenRenewalHandlerHelper;
            _oidcClient = oidcClient ?? throw new ArgumentNullException(nameof(oidcClient));
            _sessionStore = sessionStore ?? throw new ArgumentNullException(nameof(sessionStore));

            InnerHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (r, c, ch, e) => true
            };
        }



        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //if we have no access token, we try to get one with refresh
            var accessToken = await GetAccessTokenAsync(cancellationToken)
                .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(accessToken) &&
                !await RefreshTokensAsync(cancellationToken).ConfigureAwait(false))
            {
                //otherwise we fail
                return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    RequestMessage = request
                };
            }

            //so seems we have a access token, so try to use it
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _sessionStore.AccessToken);
            var response = await base.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);


            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                //happy path
                return response;
            }

            //if we get 401, we try to refresh the access token
            if (!await RefreshTokensAsync(cancellationToken).ConfigureAwait(false))
            {
                //we cant refresh
                return response;
            }


            response.Dispose(); // This 401 response will not be used for anything so is disposed to unblock the socket.

            //we refreshed the token, so we try the same request once more
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _sessionStore.AccessToken);
            return await base.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {

            var res = await _oidcTokenRenewalHandlerHelper.HandleLockedAsync(
                    () => Task.FromResult(true), cancellationToken)
                .ConfigureAwait(false);

            return res
                ? _sessionStore.AccessToken
                : null;
        }

        private async Task<bool> RefreshTokensAsync(CancellationToken cancellationToken)
        {
            var refreshToken = _sessionStore.RefreshToken;
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return false;
            }

            return await _oidcTokenRenewalHandlerHelper.HandleLockedAsync(async () =>
            {
                var response = await _oidcClient.RefreshTokenAsync(refreshToken, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                if (response.IsError)
                    throw new Exception("Could not refresh token");

                _sessionStore.AccessToken = response.AccessToken;
                if (!string.IsNullOrWhiteSpace(response.RefreshToken))
                {
                    _sessionStore.RefreshToken = response.RefreshToken;
                }

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
                finally
                {
                    _lock.Release();
                }
            }
            return false;
        }

    }

}
