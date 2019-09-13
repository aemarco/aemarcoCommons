using Newtonsoft.Json;

namespace Contracts.Messages
{
    public class EntityChangedMessage
    {
        public string Sender { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
        public int Id { get; set; }

        [JsonIgnore]
        public object EntityObject { get; set; }
    }
}
