using System;
using System.Linq;
using Newtonsoft.Json;

namespace aemarcoCommons.Toolbox.GeoTools
{
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


    internal class SunriseSunsetResponse
    {
        [JsonProperty("results")]
        public SunriseSunsetInfo Results { get; set; }
       
        
        [JsonProperty("status")]
        public string Status { get; set; }
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