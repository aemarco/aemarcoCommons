namespace Contracts.Messages
{
    public class FileServiceMessage
    {
        public string ServiceType { get; set; }
        public string EntryType { get; set; }
        public int EntryId { get; set; }

    }
}
