using Newtonsoft.Json;

namespace Contracts.Api
{
    public class JwtTokenModel
    {
        [JsonProperty("sub")]
        public string UserId { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        

        public int AdultLevel { get; set; }
        public bool IsSupervisor { get; set; }
    }
}