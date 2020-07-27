using System;
using Newtonsoft.Json;

namespace Contracts.Interfaces
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
