using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using aemarcoCommons.Extensions.TimeExtensions;

namespace aemarcoCommons.Toolbox.GeoTools
{
    public class GeoService
    {

        #region ctor

        private readonly HttpClient _geoClient;
        //public GeoService(IHttpClientFactory httpClientFactory)
        //{
        //    _geoClient = httpClientFactory.CreateClient("geoClient");
        //}

        public GeoService(HttpClient client)
        {
            _geoClient = client;
        }


        public int NumberOfCachedSunriseSunsetInfos { get; set; } = 10;
        public TimeSpan MinIntervalOfIpInfoUpdate { get; set; } = TimeSpan.FromMinutes(15);
        
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
                    _lastInInfoTimestamp.Value.IsYoungerThan(MinIntervalOfIpInfoUpdate))
                {
                    return _lastIpInfo;
                }

                using var response = await _geoClient.GetAsync("http://checkip.dyndns.org");
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
                using var response = await _geoClient.GetAsync($"https://api.sunrise-sunset.org/json?{query}");
                response.EnsureSuccessStatusCode();


                var info = await response.Content.ReadAsAsync<SunriseSunsetResponse>();
                var result = info.ToInfo(throwExceptions);

                //remember 10 result, so we don´t ask to often
                _sunriseSunsetResponses.TryAdd(query, result);
                while (_sunriseSunsetResponses.Count > NumberOfCachedSunriseSunsetInfos && 
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

