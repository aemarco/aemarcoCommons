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
        
        //optional
        public string ExceptionBase64 { get; set; }
        public string AdditionalBase64Info { get; set; }



        //conveniance
        [JsonIgnore]
        public string Exception
        {
            get => (string.IsNullOrWhiteSpace(ExceptionBase64)) 
                ? null : 
                Encoding.UTF8.GetString(Convert.FromBase64String(ExceptionBase64));
            set => ExceptionBase64 = value == null 
                ? null 
                : Convert.ToBase64String(Encoding.UTF8.GetBytes(value)); 
        }
        [JsonIgnore]
        public string AdditionalInfo
        {
            get => (string.IsNullOrWhiteSpace(AdditionalBase64Info)) 
                ? null 
                : Encoding.UTF8.GetString(Convert.FromBase64String(AdditionalBase64Info));
            set => AdditionalBase64Info = value == null 
                ? null 
                : Convert.ToBase64String(Encoding.UTF8.GetBytes(value)); 
        }
    }




}
