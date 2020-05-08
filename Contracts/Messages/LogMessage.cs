using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Contracts.Messages
{
    public class LogMessage
    {
        /// <summary>
        /// timestamp of the event
        /// </summary>
        [Required]
        public DateTimeOffset Timestamp { get; set; }
        /// <summary>
        /// name of the machine, "myGamingPc"
        /// </summary>
        [Required]
        public string Environment { get; set; }
        /// <summary>
        /// client Applications name, "WallpaperChanger" 
        /// </summary>
        [Required]
        public string App { get; set; }
        /// <summary>
        /// email of the user, "tom.turbo@gmail.com"
        /// leave this null, if its submitted to Api
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// name of the class logging this, "XyService"
        /// </summary>
        [Required]
        public string Source { get; set; }
        /// <summary>
        /// severity of the log message
        /// </summary>
        [Required]
        public LogLevel LogLevel { get; set; }
        /// <summary>
        /// message, "Operation xy failed"
        /// </summary>
        [Required]
        public string Message { get; set; }
        
        
        /// <summary>
        /// base64 encoded Exception
        /// </summary>
        public string ExceptionBase64 { get; set; }
        /// <summary>
        /// base64 encoded Additional Information
        /// </summary>
        public string AdditionalBase64Info { get; set; }






        //convenience
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
