namespace Contracts.Messages
{
    public class EntityServiceMessage
    {
        public string EntryType { get; set; }

        public string ServiceType { get; set; }

        public int EntryId { get; set; }

        public string RequestorEmail { get; set; }
    }
}
