namespace aemarcoCommons.ToolboxTypeStore.Services;

public class TypeToFileStore<T> : ITypeToFileStore<T>
    where T : class, ITypeToFileValue, new()
{

    private readonly TypeToFileSettings _settings;
    private readonly bool _isUserProtected;
    public TypeToFileStore(
        TypeToFileSettings settings)
    {
        _settings = settings;
        _isUserProtected = typeof(T).HasAttribute<UserProtectedAttribute>();
        Instance = LoadExistingOrDefault();
    }

    public T Instance { get; private set; }

    private T LoadExistingOrDefault()
    {
        var filePath = GetStorageFilePath();
        T? loadedInstance = null;

        if (File.Exists(filePath))
        {
            try
            {
                if (_isUserProtected)
                {
                    var decryptedData = ProtectedData.Unprotect(
                        File.ReadAllBytes(filePath),
                        null,
                        DataProtectionScope.CurrentUser);
                    loadedInstance = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(decryptedData), _settings.JsonSerializerOptions);
                }
                else
                {
                    loadedInstance = JsonSerializer.Deserialize<T>(File.ReadAllText(filePath), _settings.JsonSerializerOptions);
                }
            }
            catch (Exception)
            {
                // File is corrupt or unreadable, try to delete it and start fresh
                new FileInfo(filePath).Delete();
            }
        }

        // If loading failed, or if versions don't match, reset to a new instance.
        // A CurrentVersion of 0 is treated as "versioning disabled".
        if (loadedInstance is null ||
            (loadedInstance.CurrentVersion != 0 && loadedInstance.CurrentVersion != loadedInstance.Version))
        {
            return CommitReset();
        }
        return loadedInstance;
    }

    public T CommitReset()
    {
        Instance = new T
        {
            TimestampCreated = DateTimeOffset.Now
        };
        // Version must be set after creation, in case CurrentVersion logic depends on other properties
        Instance.Version = Instance.CurrentVersion;
        SaveChanges();
        return Instance;
    }

    public void SaveChanges()
    {
        Instance.TimestampSaved = DateTimeOffset.Now;
        Instance.Version = Instance.CurrentVersion;

        var filePath = GetStorageFilePath();
        if (Path.GetDirectoryName(filePath) is { } dir)
            Directory.CreateDirectory(dir);

        var jsonData = JsonSerializer.Serialize(Instance, _settings.JsonSerializerOptions);
        if (_isUserProtected)
        {
            var encryptedData = ProtectedData.Protect(
                Encoding.UTF8.GetBytes(jsonData),
                null,
                DataProtectionScope.CurrentUser);
            File.WriteAllBytes(filePath, encryptedData);
        }
        else
        {
            File.WriteAllText(filePath, jsonData);
        }
    }

    public void Dispose()
    {
        SaveChanges();
        GC.SuppressFinalize(this);
    }



    private string GetStorageFilePath()
    {
        // Check if the type itself defines a specific file path
        var instance = new T();
        if (instance.Filepath is { } typeDefinedPath)
        {
            return typeDefinedPath;
        }

        // Otherwise, construct path from settings and type name
        if (_settings.StorageDirectory is not { } storageDir)
        {
            throw new InvalidOperationException($"Storage directory must be configured in {nameof(TypeToFileSettings)} or specified in {nameof(ITypeToFileValue.Filepath)}.");
        }

        var fileName = _isUserProtected
            ? $"{typeof(T).Name}.bin"
            : $"{typeof(T).Name}.json";

        return Path.Combine(storageDir, fileName);
    }
}
