using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Contracts.Api.RequestObjects;
using Contracts.Api.ResponseObjects;
using Contracts.Messages;
using Extensions.JwtExtensions;
using Newtonsoft.Json;

namespace Toolbox.ApiTools
{
    public class WallpaperApiClient
    {
        private const string TokenEndpoint = "/Api/token";
        private const string CategoriesListEndpoint = "/Api/Category/CategoriesList";
        private const string UpdateBlacklistEndpoint = "/Api/User/UpdateBlacklist";
        private const string UpdateFavoriteEndpoint = "/Api/User/UpdateFavorite";
        private const string SubmitLogsEndpoint = "/Api/User/SubmitLogs";

        #region ctor

        private readonly HttpClient _client;
        private readonly IWallpaperApiClientSettings _wallpaperApiClientSettings;

        public WallpaperApiClient(IWallpaperApiClientSettings wallpaperApiClientSettings)
        {
            _wallpaperApiClientSettings = wallpaperApiClientSettings;
            _client = new HttpClient
            {
                BaseAddress = new Uri(_wallpaperApiClientSettings.BaseAddress)
            };
        }

        #endregion

        #region High Level

        public async Task Login(LoginReqObj loginRequest, Func<Task> onSuccess = null)
        {
            var result = await SetToken(loginRequest);
            if (result && onSuccess != null)
            {
                await onSuccess();
            }
        }

        public async Task<bool> RefreshSessionStatus()
        {
            //not logged in
            if (!(_wallpaperApiClientSettings.TokenInfo.StillValid())) return false;

            //renew token
            return await SetToken();
        }

        public async Task<List<ExtendedCategory>> GetCategories(
            Action<Exception, HttpStatusCode?> onFailure = null)
        {
            var result =
                await GetAndDeserialize<List<ExtendedCategory>>(
                        CategoriesListEndpoint, 
                        OnUnwantedResult("Get Categories failed", onFailure))
                    .ConfigureAwait(false);
            return result;
        }


        public async Task UpdateBlacklist(
            int id, 
            bool blacklisted,
            Action onSuccess = null,
            Action<Exception, HttpStatusCode?> onFailure = null)
        {
            if (await Post(
                    $"{UpdateBlacklistEndpoint}?wallId={id}&blacklisted={blacklisted}",
                    null, 
                    OnUnwantedResult("Update Blacklist failed", onFailure))
                .ConfigureAwait(false))
            {
                onSuccess?.Invoke();
            }
        }

        public async Task UpdateFavorite(
            int id, 
            bool favorite,
            Action onSuccess = null,
            Action<Exception, HttpStatusCode?> onFailure = null)
        {
            if (await Post(
                    $"{UpdateFavoriteEndpoint}?wallId={id}&favorite={favorite}", 
                    null, 
                    OnUnwantedResult("Update Favorite failed", onFailure))
                .ConfigureAwait(false))
            {
                onSuccess?.Invoke();
            }
        }


        public async Task SubmitLogs(
            List<LogMessage> logs,
            Action onSuccess = null,
            Action<Exception, HttpStatusCode?> onFailure = null)
        {
            if (await Post(
                    SubmitLogsEndpoint, 
                    logs, 
                    OnUnwantedResult("Submit logs failed", onFailure))
                .ConfigureAwait(false))
            {
                onSuccess?.Invoke();
            }
        }


        #endregion

        #region Medium Level

        public event EventHandler<UnwantedResultArgs> UnwantedResult;

        private Action<Exception, HttpStatusCode?> OnUnwantedResult(string message, Action<Exception, HttpStatusCode?> action)
        {
            return (ex, status) =>
            {
                UnwantedResult?.Invoke(this, new UnwantedResultArgs(message, ex, status));
                action?.Invoke(ex, status);
            };
        }

        private async Task<bool> SetToken(LoginReqObj loginRequest = null)
        {
            loginRequest ??= new LoginReqObj
            {
                Token = _wallpaperApiClientSettings.Token ?? string.Empty,
                Email = _wallpaperApiClientSettings.TokenInfo?.Email,
                Password = null,
                RememberMe = false,
                WindowsUser = Environment.UserName
            };
            HttpResponseMessage response = null;
            try
            {
                var request = CreateRequest(HttpMethod.Post, TokenEndpoint, loginRequest);
                response = await InvokeAsync(request);
                
                response.EnsureSuccessStatusCode();
                _wallpaperApiClientSettings.Token = await response.Content.ReadAsStringAsync();
                return true;
            }
            catch
            {
                if (response?.StatusCode == HttpStatusCode.BadRequest)
                {
                    _wallpaperApiClientSettings.Token = string.Empty;
                }
                return false;
            }
        }


        private async Task<T> GetAndDeserialize<T>(string path, Action<Exception, HttpStatusCode?> onFailure = null)
        {

            HttpResponseMessage response = null;
            try
            {
                await EnsureValidToken().ConfigureAwait(false);
                var request = CreateRequest(HttpMethod.Get, path);
                response = await InvokeAsync(request).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception ex)
            {
                onFailure?.Invoke(ex, response?.StatusCode);
                return default;
            }
        }

        private async Task<bool> Post(string path, object content, Action<Exception, HttpStatusCode?> onFailure = null)
        {
            HttpResponseMessage response = null;
            try
            {
                await EnsureValidToken().ConfigureAwait(false);

                var request = CreateRequest(HttpMethod.Post, path, content);
                response = await InvokeAsync(request).ConfigureAwait(false);
                
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                onFailure?.Invoke(ex, response?.StatusCode);
                return false;
            }
        }


        #endregion

        #region Low Level

        private async Task EnsureValidToken()
        {
            //ensure token still valid
            if (!_wallpaperApiClientSettings.TokenInfo.StillValid())
            {
                throw new AuthenticationException("Invalid Token");
            }

            if (_wallpaperApiClientSettings.TokenInfo.ShouldBeRenewed(30))
            {
                //renew token
                if (!await SetToken().ConfigureAwait(false))
                {
                    throw new AuthenticationException("Failed renewing Token");
                }
            }
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string path, object content = null)
        {
            var request = new HttpRequestMessage(method, path);
            if (content != null)
            {
                request.Content =
                    new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            }
            return request;
        }

        private async Task<HttpResponseMessage> InvokeAsync(HttpRequestMessage request)
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _wallpaperApiClientSettings.Token);
            var response = await _client.SendAsync(request)
                .ConfigureAwait(false);
            return response;
        }

        #endregion
    }
}
