using aemarcoCommons.Extensions.TimeExtensions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace aemarcoCommons.Toolbox.GeoTools
{
    public class GeoService
    {
        #region ctor

        private readonly IGeoServiceSettings _geoServiceSettings;
        private readonly HttpClient _geoClient;


        public GeoService()
            : this(new GeoServiceSettings(), new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (r, c, chain, errors) => true
            }))
        { }

        public GeoService(IGeoServiceSettings geoServiceSettings)
            : this(geoServiceSettings, new HttpClient())
        { }

        public GeoService(IHttpClientFactory httpClientFactory)
            : this(new GeoServiceSettings(), httpClientFactory)
        { }

        public GeoService(IGeoServiceSettings geoServiceSettings, IHttpClientFactory httpClientFactory)
            : this(geoServiceSettings, httpClientFactory.CreateClient(nameof(GeoService)))
        { }

        private GeoService(IGeoServiceSettings geoServiceSettings, HttpClient client)
        {
            _geoServiceSettings = geoServiceSettings;
            _geoClient = client;

            if (_geoServiceSettings.NumberOfCachedSunriseSunsetInfos < 0)
                throw new ArgumentException("NumberOfCachedSunriseSunsetInfos must be >= 0", nameof(geoServiceSettings));
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

                using (var response = await _geoClient.GetAsync("http://checkip.dyndns.org"))
                {
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
            bool throwExceptions = false,
            bool byPassCache = false)
        {
            //reduce precision to get caching take more effect
            latitude = Convert.ToSingle(Math.Round(latitude, 3));
            longitude = Convert.ToSingle(Math.Round(longitude, 3));
            //default to today
            if (date == null) date = DateTimeOffset.Now; //2020-10-03


            var query = $"lat={latitude}&lng={longitude}&date={date.Value.Date:yyyy-MM-dd}&formatted=0";

            //return cached result if already present
            if (!byPassCache && _sunriseSunsetResponses.TryGetValue(query, out SunriseSunsetInfo cached))
                return cached;

            try
            {
                using (HttpResponseMessage response = await _geoClient
                           .GetAsync($"https://api.sunrise-sunset.org/json?{query}"))
                {
                    response.EnsureSuccessStatusCode();


                    var info = await response.Content.ReadAsAsync<SunriseSunsetResponse>();
                    var result = info.ToInfo(throwExceptions);

                    //remember configured number of result, so we don´t ask to often
                    _sunriseSunsetResponses.TryAdd(query, result);
                    while (_sunriseSunsetResponses.Count > _geoServiceSettings.NumberOfCachedSunriseSunsetInfos)
                    {
                        _sunriseSunsetResponses.TryRemove(_sunriseSunsetResponses.Keys.First(), out _);
                    }
                    return result;
                }
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

