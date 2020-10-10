using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using aemarcoCommons.Extensions.TimeExtensions;

namespace aemarcoCommons.Toolbox.GeoTools
{
    public class GeoService
    {
        #region ctor

        private readonly IGeoServiceSettings _geoServiceSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _geoClient;
        private HttpClient GeoClient => _httpClientFactory?.CreateClient(nameof(GeoService)) ?? _geoClient;

        public GeoService(IHttpClientFactory httpClientFactory)
            : this(new GeoServiceSettings(), httpClientFactory)
        { }

        public GeoService(IGeoServiceSettings geoServiceSettings, IHttpClientFactory httpClientFactory)
        {
            _geoServiceSettings = geoServiceSettings;
            _httpClientFactory = httpClientFactory;
        }

       
        public GeoService()
            :this(new GeoServiceSettings())
        { }

        public GeoService(IGeoServiceSettings geoServiceSettings)
        {
            _geoServiceSettings = geoServiceSettings;
            _geoClient = new HttpClient();
        }

        #endregion

        #region IpInfo

        private string _lastIpInfo;
        private DateTimeOffset? _lastInInfoTimestamp;
        public async Task<string> GetIpInfo(bool throwExceptions = false)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_lastIpInfo) &&
                    _lastInInfoTimestamp.HasValue &&
                    _lastInInfoTimestamp.Value.IsYoungerThan(_geoServiceSettings.MinIntervalOfIpInfoUpdate))
                {
                    return _lastIpInfo;
                }

                using var response = await GeoClient.GetAsync("http://checkip.dyndns.org");
                response.EnsureSuccessStatusCode();

                var info = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(info)) return null;

                var match = Regex.Match(info, @"(?:\d{1,3}.){3}.\d{1,3}");

                var result = match.Success
                    ? match.Value
                    : null;

                _lastIpInfo = result;
                _lastInInfoTimestamp = DateTimeOffset.Now;

                return result;
            }
            catch
            {
                if (throwExceptions) throw;
                return null;
            }
        }

        #endregion

        #region SunriseSunset

        private readonly ConcurrentDictionary<string, SunriseSunsetInfo> _sunriseSunsetResponses = new ConcurrentDictionary<string, SunriseSunsetInfo>();
        
        public async Task<SunriseSunsetInfo> GetSunriseSunsetInfoInfo(
            float latitude,
            float longitude,
            DateTimeOffset? date = null,
            bool throwExceptions = false)
        {
            date ??= DateTimeOffset.Now; //2020-10-03
            var query = $"lat={latitude}&lng={longitude}&date={date.Value.Date:yyyy-MM-dd}&formatted=0";

            //return cached result if already present
            if (_sunriseSunsetResponses.TryGetValue(query, out var cached)) return cached;

            try
            {
                using var response = await GeoClient.GetAsync($"https://api.sunrise-sunset.org/json?{query}");
                response.EnsureSuccessStatusCode();


                var info = await response.Content.ReadAsAsync<SunriseSunsetResponse>();
                var result = info.ToInfo(throwExceptions);

                //remember 10 result, so we don´t ask to often
                _sunriseSunsetResponses.TryAdd(query, result);
                while (_sunriseSunsetResponses.Count > _geoServiceSettings.NumberOfCachedSunriseSunsetInfos && 
                       _sunriseSunsetResponses.TryRemove(_sunriseSunsetResponses.Keys.First(), out _)) { }

                return result;
            }
            catch
            {
                if (throwExceptions) throw;
                return null;
            }
        }

        #endregion

    }
}

