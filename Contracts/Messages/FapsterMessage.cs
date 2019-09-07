using System;

namespace Contracts.Messages
{
    public class FapsterMessage
    {
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
        public bool Running { get; set; }
        public int MinAdult { get; set; } = 0;
        public int MaxAdult { get; set; } = 0;
    }
}
