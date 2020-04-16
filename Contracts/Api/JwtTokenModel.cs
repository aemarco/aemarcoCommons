using System;
using System.Globalization;
using Contracts.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Contracts.Api
{
    public class JwtTokenModel : IUserInfo
    {
        [JsonProperty("iat")]
        [JsonConverter(typeof(SecondEpochConverter))]
        public DateTimeOffset IssuedAt { get; set; } = DateTimeOffset.MinValue;
        [JsonProperty("exp")]
        [JsonConverter(typeof(SecondEpochConverter))]
        public DateTimeOffset ValidUntil { get; set; } = DateTimeOffset.MinValue;
        [JsonProperty("sub")]
        public string UserId { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("stamp")]
        public string SecurityStamp { get; set; }
        public int AdultLevel { get; set; }
        public bool IsSupervisor { get; set; }
    }

    public class SecondEpochConverter : DateTimeConverterBase
    {
        private static readonly DateTimeOffset _epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(((DateTimeOffset)value - _epoch).TotalSeconds.ToString(CultureInfo.InvariantCulture));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            return _epoch.AddSeconds((long)reader.Value);
        }
    }





}