using System;
using System.IO;
using Contracts.Interfaces;
using Extensions.netExtensions;
using Newtonsoft.Json;

namespace Toolbox.SerializationTools
{
    public class JsonTypeToFileStore<T> : ITypeToFileStore<T>
        where T : class, ITypeToFileValue, new()
    {
        public JsonTypeToFileStore()
        {
            var file = new T().Filepath;
            //loads store from file possibly, but defaults back to new store
            if (File.Exists(file))
            {
                try
                {
                    Instance = JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
                }
                catch
                {
                    new FileInfo(file).TryDelete();
                }
            }
            Instance ??= new T()
            {
                TimestampCreated = DateTimeOffset.Now
            };
        }

        public T Instance { get; private set; }

        public void SaveChanges()
        {
            Instance.TimestampSaved = DateTimeOffset.Now;
            File.WriteAllText(Instance.Filepath, JsonConvert.SerializeObject(Instance, Formatting.Indented));
        }

        public void CommitReset()
        {
            Instance = new T()
            {
                TimestampCreated = DateTimeOffset.Now
            };
            SaveChanges();
        }

        public void Dispose()
        {
            SaveChanges();
        }
    }
}
