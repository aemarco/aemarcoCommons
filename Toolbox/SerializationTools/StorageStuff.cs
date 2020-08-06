using System;
using Newtonsoft.Json;

namespace Toolbox.SerializationTools
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
        DateTimeOffset TimestampCreated { get; set; }
        DateTimeOffset TimestampSaved { get; set; }
    }
}
