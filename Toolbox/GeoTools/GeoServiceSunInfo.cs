using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace aemarcoCommons.Toolbox.GeoTools
{

    public partial interface IGeoServiceSettings
    {
        int NumberOfCachedSunriseSunsetInfos { get; }
    }

    internal partial class GeoServiceSettings
    {
        public int NumberOfCachedSunriseSunsetInfos { get; set; } = 10;
    }


    public partial class GeoService
    {

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

    }

    internal class SunriseSunsetResponse
    {
        [JsonProperty("results")]
        public SunriseSunsetInfo Results { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }


    public class SunriseSunsetInfo
    {
        [JsonProperty("sunrise")]
        public DateTimeOffset Sunrise { get; set; }

        [JsonProperty("sunset")]
        public DateTimeOffset Sunset { get; set; }

        [JsonProperty("solar_noon")]
        public DateTimeOffset SolarNoon { get; set; }

        [JsonProperty("day_length")]
        [JsonConverter(typeof(TimeSpanSecondsConverter))]
        public TimeSpan DayLength { get; set; }

        [JsonProperty("civil_twilight_begin")]
        public DateTimeOffset CivilTwilightBegin { get; set; }

        [JsonProperty("civil_twilight_end")]
        public DateTimeOffset CivilTwilightEnd { get; set; }

        [JsonProperty("nautical_twilight_begin")]
        public DateTimeOffset NauticalTwilightBegin { get; set; }

        [JsonProperty("nautical_twilight_end")]
        public DateTimeOffset NauticalTwilightEnd { get; set; }

        [JsonProperty("astronomical_twilight_begin")]
        public DateTimeOffset AstronomicalTwilightBegin { get; set; }

        [JsonProperty("astronomical_twilight_end")]
        public DateTimeOffset AstronomicalTwilightEnd { get; set; }
    }

    public class TimeSpanSecondsConverter : JsonConverter<TimeSpan>
    {
        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
        {
            writer.WriteValue(((int)value.TotalSeconds).ToString());
        }

        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var incoming = (long)(reader.Value ?? 0);
            return TimeSpan.FromSeconds(incoming);
        }
    }









    internal static class SunriseSunsetExtensions
    {
        public static SunriseSunsetInfo ToInfo(this SunriseSunsetResponse response, bool throwExceptions = false)
        {
            //if okay, we return info
            if (response.Status == "OK")
            {
                foreach (var propInfo in typeof(SunriseSunsetInfo).GetProperties()
                             .Where(x => x.PropertyType == typeof(DateTimeOffset)))
                {
                    var val = (DateTimeOffset)propInfo.GetValue(response.Results);
                    propInfo.SetValue(response.Results, val.ToLocalTime());
                }
                return response.Results;
            }

            //if it should throw, we throw with status as message
            if (throwExceptions) throw new Exception(response.Status);

            //else
            return null;
        }

    }





}
