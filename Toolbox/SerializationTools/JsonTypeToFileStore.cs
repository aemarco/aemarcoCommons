using aemarcoCommons.Extensions.FileExtensions;
using Newtonsoft.Json;
using System;
using System.IO;

namespace aemarcoCommons.Toolbox.SerializationTools
{
    public interface IJsonTypeToFileSettings
    {
        string StorageDirectory { get; }
        Formatting Formatting { get; }
    }
    public class JsonTypeToFileSettings : IJsonTypeToFileSettings
    {
        public string StorageDirectory { get; set; }
        public Formatting Formatting { get; set; }
    }


    public class JsonTypeToFileStore<T> : ITypeToFileStore<T>
        where T : class, ITypeToFileValue, new()
    {

        private readonly IJsonTypeToFileSettings _settings;
        public JsonTypeToFileStore()
            : this(new JsonTypeToFileSettings())
        { }
        // ReSharper disable once MemberCanBePrivate.Global
        public JsonTypeToFileStore(IJsonTypeToFileSettings settings)
        {
            _settings = settings;

            LoadExistingOrDefault();
        }

        public T Instance { get; private set; }

        public void SaveChanges()
        {
            Instance.TimestampSaved = DateTimeOffset.Now;
            Instance.Version = Instance.CurrentVersion;

            var filePath = GetStorageFilePath();
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new Exception("FilePath unclear"));

            File.WriteAllText(
                filePath,
                JsonConvert.SerializeObject(Instance, _settings.Formatting));
        }

        public T CommitReset()
        {
            Instance = new T
            {
                TimestampCreated = DateTimeOffset.Now,
            };
            Instance.Version = Instance.CurrentVersion; //save the version we have
            SaveChanges();
            return Instance;
        }

        public void Dispose()
        {
            SaveChanges();
        }


        /// <summary>
        /// Possibly loads Instance from file or defaults back to new Instance
        /// </summary>
        /// <exception cref="Exception">when neither config nor instance provide folder and or name for file</exception>
        private void LoadExistingOrDefault()
        {
            var file = GetStorageFilePath();

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

            if (Instance is null ||
                (Instance.CurrentVersion != 0 && Instance.CurrentVersion != Instance.Version))
            {
                CommitReset();
            }
        }

        /// <summary>
        /// Figure out which path the Instance should be saved to
        /// </summary>
        /// <returns>The absolute file path for the instance</returns>
        /// <exception cref="Exception">when neither config nor instance provide folder and or name for file</exception>
        private string GetStorageFilePath()
        {
            //we primarily use interface
            var file = new T().Filepath;

            //directory
            var dir = Path.GetDirectoryName(file);
            if (string.IsNullOrWhiteSpace(dir))
                dir = _settings.StorageDirectory ?? throw new Exception("Storage directory could not be determined. Either configure general path in IJsonTypeToFileSettings or specify in ITypeToFileValue.");


            var fileName = Path.GetFileName(file);
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = $"{typeof(T).Name}.json";

            var result = Path.Combine(dir, fileName);
            return result;
        }


    }


}
