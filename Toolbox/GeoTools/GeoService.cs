using aemarcoCommons.Extensions.TimeExtensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public async Task<string> GetIpInfo(
            bool throwExceptions = false,
            TimeSpan? maximumInfoAge = null)
        {
            maximumInfoAge = maximumInfoAge ?? _geoServiceSettings.MinIntervalOfIpInfoUpdate;
            if (!string.IsNullOrWhiteSpace(_lastIpInfo) &&
                _lastInInfoTimestamp.HasValue &&
                _lastInInfoTimestamp.Value.IsYoungerThan(maximumInfoAge.Value))
            {
                return _lastIpInfo;
            }
            string result = null;


            var errors = new List<Exception>(2);
            try
            {
                using (var response = await _geoClient
                           .GetAsync("https://api.ipify.org/")
                           .ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    var ipText = await response.Content
                        .ReadAsStringAsync()
                        .ConfigureAwait(false);
                    if (IPAddress.TryParse(ipText, out IPAddress test))
                        result = test.ToString();
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }
            if (string.IsNullOrWhiteSpace(result))
            {
                try
                {
                    using (var response = await _geoClient
                               .GetAsync("http://checkip.dyndns.org")
                               .ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        var info = await response.Content.ReadAsStringAsync();
                        var match = Regex.Match(info ?? string.Empty, @"(?:\d{1,3}.){3}.\d{1,3}");
                        if (match.Success && IPAddress.TryParse(match.Value, out IPAddress test))
                            result = test.ToString();
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                    if (throwExceptions)
                        throw new AggregateException(errors);
                }
            }



            if (result != null)
            {
                _lastIpInfo = result;
                _lastInInfoTimestamp = DateTimeOffset.Now;
            }
            return result;
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

