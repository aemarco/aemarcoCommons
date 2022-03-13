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
    public interface ISessionHandler
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
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private readonly OidcClient _oidcClient;
        private readonly ISessionHandler _sessionHandler;

        private bool _disposed;
        private TimeSpan Timeout { get; } = TimeSpan.FromSeconds(5);

        public OidcTokenRenewalHandler(
            OidcClient oidcClient,
            ISessionHandler sessionHandler)
        {
            _oidcClient = oidcClient ?? throw new ArgumentNullException(nameof(oidcClient));
            _sessionHandler = sessionHandler ?? throw new ArgumentNullException(nameof(sessionHandler));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //if we have no access token, we try to get one with refresh
            var accessToken = await GetAccessTokenAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(accessToken) && !await RefreshTokensAsync(cancellationToken))
            {
                //otherwise we fail
                return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    RequestMessage = request
                };
            }

            //so seems we have a access token, so try to use it
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _sessionHandler.AccessToken);
            var response = await base.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);


            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                //happy path
                return response;
            }

            //if we get 401, we try to refresh the access token
            if (!await RefreshTokensAsync(cancellationToken))
            {
                //we cant refresh
                return response;
            }


            response.Dispose(); // This 401 response will not be used for anything so is disposed to unblock the socket.

            //we refreshed the token, so we try the same request once more
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _sessionHandler.AccessToken);
            return await base.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                _lock.Dispose();
            }

            base.Dispose(disposing);
        }

        private async Task<bool> RefreshTokensAsync(CancellationToken cancellationToken)
        {
            var refreshToken = _sessionHandler.RefreshToken;
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return false;
            }

            if (await _lock.WaitAsync(Timeout, cancellationToken)
                .ConfigureAwait(false))
            {
                try
                {
                    var response = await _oidcClient.RefreshTokenAsync(refreshToken, cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

                    if (!response.IsError)
                    {
                        _sessionHandler.AccessToken = response.AccessToken;
                        if (!string.IsNullOrWhiteSpace(response.RefreshToken))
                        {
                            _sessionHandler.RefreshToken = response.RefreshToken;
                        }
                        return true;
                    }
                }
                finally
                {
                    _lock.Release();
                }
            }

            return false;
        }

        private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            if (await _lock.WaitAsync(Timeout, cancellationToken)
                .ConfigureAwait(false))
            {
                try
                {
                    return _sessionHandler.AccessToken;
                }
                finally
                {
                    _lock.Release();
                }
            }

            return null;
        }
    }
}
