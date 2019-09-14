using Newtonsoft.Json;

namespace Contracts.Messages
{
    public class SpeechMessage
    {
        public string Target { get; set; }
        public string MessageType { get; set; }


        public string Key { get; set; }
        public int Value { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
