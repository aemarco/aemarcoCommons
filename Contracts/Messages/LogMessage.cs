using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

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
        public Dictionary<string, string> ExceptionDict { get; set; }



        [JsonIgnore]
        public Exception Exception
        {
            set
            {
                if (value != null)
                {
                    ExceptionDict = new Dictionary<string, string>
                    {
                        {"Type", value.GetType().ToString()},
                        {"Message", value.Message},
                        {"StackTrace", value.StackTrace}
                    };
                    foreach (DictionaryEntry data in value.Data)
                        ExceptionDict.Add(data.Key.ToString(), data.Value.ToString());

                }
            }

        }

    }




}
