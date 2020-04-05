using Newtonsoft.Json;
using System;
using System.Text;

namespace Contracts.Messages
{
    public class LogMessage
    {
        public DateTimeOffset Timestamp { get; set; }
        public string Environment { get; set; }
        public string App { get; set; }
        public string Source { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        //optionals
        public string ExceptionBase64 { get; set; }
        public string AdditionalBase64Info { get; set; }



        //conveniance
        [JsonIgnore]
        public string Exception
        {
            get
            {
                return (String.IsNullOrWhiteSpace(ExceptionBase64)) ? null : Encoding.UTF8.GetString(Convert.FromBase64String(ExceptionBase64));
            }
            set
            {
                if (value == null) ExceptionBase64 = null;
                else ExceptionBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
            }
        }
        [JsonIgnore]
        public string AdditionalInfo
        {
            get
            {
                return (String.IsNullOrWhiteSpace(AdditionalBase64Info)) ? null : Encoding.UTF8.GetString(Convert.FromBase64String(AdditionalBase64Info));
            }
            set
            {
                if (value == null) AdditionalBase64Info = null;
                else AdditionalBase64Info = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
            }
        }
    }




}
