namespace Contracts.Api.ResponseObjects
{
    public class LogSetting
    {
        /// <summary>
        /// if logging to the api should be enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// the minimum level, which should be logged to the api
        /// </summary>
        public LogLevel MinLevel { get; set; }

    }

}
