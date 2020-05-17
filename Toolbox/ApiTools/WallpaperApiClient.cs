using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Contracts.Api;
using Contracts.Api.RequestObjects;
using Contracts.Api.ResponseObjects;
using Contracts.Interfaces;
using Contracts.Messages;
using Extensions.JwtExtensions;
using Newtonsoft.Json;

namespace Toolbox.ApiTools
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WallpaperApiClient : ISingleton
    {
        private const string TokenEndpoint = "/Api/token";
        private const string CategoriesListEndpoint = "/Api/Category/CategoriesList";
        private const string FindGirlsBySearchEndpoint = "/Api/Girl/FindBySearch";
        private const string GetAllGirlEndpoint = "/Api/Girl/GetAll";
        private const string UpdateBlacklistEndpoint = "/Api/User/UpdateBlacklist";
        private const string UpdateFavoriteEndpoint = "/Api/User/UpdateFavorite";
        private const string GetLogSettingEndpoint = "/Api/Log/GetLogSetting";
        private const string SubmitLogsEndpoint = "/Api/Log/SubmitLogs";
        private const string ResolveWallpaperFilterRequestEndpoint = "/Api/Wallpaper/ResolveWallpaperFilterRequest";

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

        private JwtTokenModel _tokenInfo;
        public JwtTokenModel TokenInfo => _tokenInfo ??= _wallpaperApiClientSettings.Token.ToJwtTokenModel();
        public HttpClient Client => _client;


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
            if (!(TokenInfo.StillValid())) return false;

            //renew token
            return await SetToken();
        }

        public Task<List<ExtendedCategory>> GetCategories(
            Action<Exception, HttpStatusCode?> onFailure = null, 
            CancellationToken cancellationToken = new CancellationToken())
        {
            var result =
                 GetAndDeserialize<List<ExtendedCategory>>(
                    CategoriesListEndpoint,
                    cancellationToken,
                    OnUnwantedResult("Get Categories failed", onFailure));
                    
            return result;
        }


        public Task<List<ExtendedGirl>> FindGirlsBySearch(
            string search,  
            Action<Exception, HttpStatusCode?> onFailure = null, 
            CancellationToken cancellationToken = new CancellationToken())
        {
            var result =
                PostAndDeserialize<List<ExtendedGirl>>(
                        $"{FindGirlsBySearchEndpoint}?search={search}",
                        OnUnwantedResult("Failed to search for Girls", onFailure),
                        cancellationToken);

            return result;
        }

        public Task<List<ExtendedGirl>> GetAllGirls(
            Action<Exception, HttpStatusCode?> onFailure = null, 
            CancellationToken cancellationToken = new CancellationToken())
        {
            var result =
                GetAndDeserialize<List<ExtendedGirl>>(
                    GetAllGirlEndpoint,
                    cancellationToken,
                    OnUnwantedResult("Failed to get Girls", onFailure));
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
                    CancellationToken.None,
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
                    CancellationToken.None,
                    OnUnwantedResult("Update Favorite failed", onFailure))
                .ConfigureAwait(false))
            {
                onSuccess?.Invoke();
            }
        }


        public Task<LogSetting> GetLogSetting(
            string environment, 
            string app,
            Action<Exception, HttpStatusCode?> onFailure = null, 
            CancellationToken cancellationToken = new CancellationToken())
        {
            var result =
                GetAndDeserialize<LogSetting>(
                    $"{GetLogSettingEndpoint}?environment={environment}&app={app}",
                    cancellationToken,
                    OnUnwantedResult("Get LogSetting failed", onFailure));
            return result;
        }


        public async Task SubmitLogs(
            List<LogMessage> logs,
            Action onSuccess = null,
            Action<Exception, HttpStatusCode?> onFailure = null)
        {
            if (await Post(
                    SubmitLogsEndpoint, 
                    logs, 
                    CancellationToken.None,
                    OnUnwantedResult("Submit logs failed", onFailure))
                .ConfigureAwait(false))
            {
                onSuccess?.Invoke();
            }
        }


        public Task<WallpaperFilterResponse> ResolveWallpaperFilterRequest(
            WallpaperFilterRequest request, 
            Action<Exception, HttpStatusCode?> onFailure = null, 
            CancellationToken cancellationToken = new CancellationToken())
        {
            var result = PostAndDeserialize<WallpaperFilterResponse>(
                ResolveWallpaperFilterRequestEndpoint, 
                    request,
                    cancellationToken,
                    OnUnwantedResult("Resolve WallpaperFilter Request failed", onFailure));

            return result;
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
                Token = _wallpaperApiClientSettings.Token ?? null,
                Email = TokenInfo?.Email,
                Password = null,
                WindowsUser = Environment.UserName
            };
            HttpResponseMessage response = null;
            try
            {
                var request = CreateRequest(HttpMethod.Post, TokenEndpoint, loginRequest);
                response = await InvokeAsync(request, CancellationToken.None);
                
                response.EnsureSuccessStatusCode();
                _wallpaperApiClientSettings.Token = await response.Content.ReadAsStringAsync();
                _tokenInfo = null;
                return true;
            }
            catch
            {
                if (response?.StatusCode == HttpStatusCode.BadRequest)
                {
                    _wallpaperApiClientSettings.Token = string.Empty;
                    _tokenInfo = null;
                }
                return false;
            }
        }


        private async Task<T> GetAndDeserialize<T>(string path, CancellationToken cancellationToken, Action<Exception, HttpStatusCode?> onFailure = null)
        {

            HttpResponseMessage response = null;
            try
            {
                await EnsureValidToken().ConfigureAwait(false);
                var request = CreateRequest(HttpMethod.Get, path);
                response = await InvokeAsync(request, cancellationToken).ConfigureAwait(false);

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

        private async Task<T> PostAndDeserialize<T>(string path, object reqContent, CancellationToken cancellationToken, Action<Exception, HttpStatusCode?> onFailure = null)
        {

            HttpResponseMessage response = null;
            try
            {
                await EnsureValidToken().ConfigureAwait(false);
                var request = CreateRequest(HttpMethod.Post, path, reqContent);
                response = await InvokeAsync(request,cancellationToken).ConfigureAwait(false);

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


        private async Task<bool> Post(string path, object content, CancellationToken cancellationToken, Action<Exception, HttpStatusCode?> onFailure = null)
        {
            HttpResponseMessage response = null;
            try
            {
                await EnsureValidToken().ConfigureAwait(false);

                var request = CreateRequest(HttpMethod.Post, path, content);
                response = await InvokeAsync(request, cancellationToken).ConfigureAwait(false);
                
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
            if (!TokenInfo.StillValid())
            {
                throw new AuthenticationException("Invalid Token");
            }

            if (TokenInfo.ShouldBeRenewed(30))
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

        private async Task<HttpResponseMessage> InvokeAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _wallpaperApiClientSettings.Token);
            var response = await _client.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);
            return response;
        }

        #endregion
    }
}
