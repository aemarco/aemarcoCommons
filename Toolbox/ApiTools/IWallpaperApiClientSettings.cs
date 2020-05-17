using Contracts.Api;

namespace Toolbox.ApiTools
{
    public interface IWallpaperApiClientSettings
    {
        string BaseAddress { get; }
        string Token { get; set; }
    }
}