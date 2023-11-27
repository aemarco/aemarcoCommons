using aemarcoCommons.Extensions.FileExtensions;
using Newtonsoft.Json;
using System;
using System.IO;

namespace aemarcoCommons.Toolbox.SerializationTools
{
    public class JsonTypeToFileStore<T> : ITypeToFileStore<T>
        where T : class, ITypeToFileValue, new()
    {
        public JsonTypeToFileStore()
        {
            //determine where file should be
            var file = new T().Filepath;
            //loads store from file possibly, but defaults back to new store
            if (File.Exists(file))
            {
                try
                {
                    Instance = JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
                    if (Instance.CurrentVersion != Instance.Version)
                    {
                        CommitReset();
                    }
                }
                catch
                {
                    new FileInfo(file).TryDelete();
                }
            }

            if (Instance is null)
            {
                CommitReset();
            }
        }

        public T Instance { get; private set; }

        public void SaveChanges()
        {
            Instance.TimestampSaved = DateTimeOffset.Now;
            var di = new FileInfo(Instance.Filepath).Directory;
            if (di != null && !di.Exists)
                di.Create();

            File.WriteAllText(Instance.Filepath, JsonConvert.SerializeObject(Instance, Formatting.Indented));
        }

        public T CommitReset()
        {
            Instance = new T()
            {
                TimestampCreated = DateTimeOffset.Now,
            };
            Instance.Version = Instance.CurrentVersion; //save version we have
            SaveChanges();
            return Instance;
        }

        public void Dispose()
        {
            SaveChanges();
        }
    }
}
