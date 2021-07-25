using Newtonsoft.Json;
using System;

namespace aemarcoCommons.Toolbox.SerializationTools
{
    public interface ITypeToFileStore<out T> : IDisposable
        where T : class, ITypeToFileValue, new()
    {
        T Instance { get; }
        T CommitReset();
        void SaveChanges();
    }

    public interface ITypeToFileValue
    {
        [JsonIgnore]
        string Filepath { get; }
        [JsonIgnore]
        int CurrentVersion { get; }


        int Version { get; set; }
        DateTimeOffset TimestampCreated { get; set; }
        DateTimeOffset TimestampSaved { get; set; }
    }
}
