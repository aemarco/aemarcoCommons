using aemarcoCommons.Extensions;
using aemarcoCommons.Extensions.FileExtensions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

#pragma warning disable IDE0130
namespace aemarcoCommons.Toolbox.SerializationTools;
#pragma warning restore IDE0130

[Obsolete("Use ToolboxTypeStore instead.")]
public interface IJsonTypeToFileSettings
{
    string StorageDirectory { get; }
    Formatting Formatting { get; }
}

[Obsolete("Use ToolboxTypeStore instead.")]
public class JsonTypeToFileSettings : IJsonTypeToFileSettings
{
    public string StorageDirectory { get; set; }
    public Formatting Formatting { get; set; }
}

[Obsolete("Use ToolboxTypeStore instead.")]
public class JsonTypeToFileStore<T> : ITypeToFileStore<T>
    where T : class, ITypeToFileValue, new()
{

    private readonly IJsonTypeToFileSettings _settings;
    private readonly bool _isUserProtected;
    public JsonTypeToFileStore()
        : this(new JsonTypeToFileSettings())
    { }
    // ReSharper disable once MemberCanBePrivate.Global
    public JsonTypeToFileStore(IJsonTypeToFileSettings settings)
    {
        _settings = settings;
        _isUserProtected = typeof(T).HasAttribute<UserProtectedAttribute>();

        LoadExistingOrDefault();
    }

    public T Instance { get; private set; }

    public void SaveChanges()
    {
        Instance.TimestampSaved = DateTimeOffset.Now;
        Instance.Version = Instance.CurrentVersion;

        var filePath = GetStorageFilePath(false);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new Exception("FilePath unclear"));


        if (_isUserProtected && OperatingSystem.IsWindows())
        {
            filePath = GetStorageFilePath(true);
            File.WriteAllBytes(
                filePath,
                ProtectedData.Protect(
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Instance)),
                    null,
                    DataProtectionScope.CurrentUser));
        }
        else
        {
            File.WriteAllText(
                filePath,
                JsonConvert.SerializeObject(Instance, _settings.Formatting));
        }
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


        if (_isUserProtected && OperatingSystem.IsWindows())
        {
            var file = GetStorageFilePath(true);
            if (File.Exists(file))
            {
                try
                {
                    string json = Encoding.UTF8.GetString(ProtectedData.Unprotect(
                        File.ReadAllBytes(file),
                        null,
                        DataProtectionScope.CurrentUser));
                    Instance = JsonConvert.DeserializeObject<T>(json);
                }
                catch
                {
                    new FileInfo(file).TryDelete();
                }
            }
        }
        else
        {
            var file = GetStorageFilePath(false);
            if (File.Exists(file))
            {
                try
                {
                    string json = File.ReadAllText(file);
                    Instance = JsonConvert.DeserializeObject<T>(json);
                }
                catch
                {
                    new FileInfo(file).TryDelete();
                }
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
    private string GetStorageFilePath(bool isProtected)
    {
        //we primarily use interface
        var file = new T().Filepath;

        //directory (1. from file, 2. from settings)
        var dir = file != null
            ? new FileInfo(file).DirectoryName ?? throw new Exception("Could not locate file")
            : _settings.StorageDirectory ?? throw new Exception("Storage directory could not be determined. Either configure general path in IJsonTypeToFileSettings or specify in ITypeToFileValue.");

        //fileName (1. from file, 2. from type)
        var fileName = file != null
            ? Path.GetFileName(file)
            : isProtected
                ? $"{typeof(T).Name}.bin"
                : $"{typeof(T).Name}.json";

        var result = Path.Combine(dir, fileName);
        return result;
    }





}