namespace aemarcoCommons.ToolboxTypeStore.Services;

public class TypeToFileStore<T> : ITypeToFileStore<T>
    where T : class, ITypeToFileValue, new()
{

    private readonly TypeToFileSettings _settings;
    private readonly bool _isUserProtected;
    private readonly int _currentVersion;
    private readonly ILogger<TypeToFileStore<T>> _logger;
    public TypeToFileStore(
        TypeToFileSettings settings,
        ILogger<TypeToFileStore<T>> logger)
    {
        _settings = settings;
        var typeSettings = typeof(T).GetCustomAttribute<TypeToFileSettingsAttribute>();
        _isUserProtected = typeSettings?.IsUserProtected ?? false;
        _currentVersion = typeSettings?.Version ?? 0;
        _logger = logger;
        Instance = LoadExistingOrDefault();
    }

    public T Instance { get; private set; }
    public DateTimeOffset TimestampCreated { get; private set; }
    public DateTimeOffset? TimestampSaved { get; private set; }

    private T LoadExistingOrDefault()
    {
        var filePath = GetStorageFilePath();
        var fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
            return CommitReset();

        // --- Try to load from file ---
        T? loadedInstance;
        try
        {
            var jsonContent = _isUserProtected
                ? Encoding.UTF8.GetString(
                    ProtectedData.Unprotect(
                        File.ReadAllBytes(filePath),
                        null,
                        DataProtectionScope.CurrentUser))
                : File.ReadAllText(filePath);

            var node = JsonNode.Parse(jsonContent, new JsonNodeOptions { PropertyNameCaseInsensitive = true });
            if (node is JsonObject jObj)
            {
                // Extract and remove version
                if (jObj.TryGetPropertyValue("_version", out var versionNode) && versionNode != null)
                {
                    var versionFromFile = versionNode.GetValue<int>();
                    if (_currentVersion != 0 && versionFromFile != _currentVersion)
                    {
                        _logger.LogWarning($"Loaded file for type {typeof(T).Name} has version {versionFromFile}, but current app version is {_currentVersion}. Resetting to default.");
                        return CommitReset();
                    }
                    jObj.Remove("_version");
                }
                // Extract and remove timestamps
                if (jObj.TryGetPropertyValue("_timestampCreated", out var tsCreatedNode) && tsCreatedNode != null)
                {
                    TimestampCreated = tsCreatedNode.GetValue<DateTimeOffset>();
                    jObj.Remove("_timestampCreated");
                }
                if (jObj.TryGetPropertyValue("_timestampSaved", out var tsSavedNode) && tsSavedNode != null)
                {
                    TimestampSaved = tsSavedNode.GetValue<DateTimeOffset?>();
                    jObj.Remove("_timestampSaved");
                }
            }
            loadedInstance = JsonSerializer.Deserialize<T>(node?.ToJsonString() ?? jsonContent, _settings.JsonSerializerOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error loading file for type {typeof(T).Name}. File: {filePath}. Deleting corrupt file.");

            loadedInstance = null;
            fileInfo.TryDelete(); // File is corrupt or unreadable
        }

        if (loadedInstance is null)
        {//could not load or deserialize, so reset to default
            _logger.LogWarning($"Loaded instance for type {typeof(T).Name} is null. Resetting to default.");
            return CommitReset();
        }

        // Happy path: loadedInstance is valid, versions match or versioning is not active.
        _logger.LogInformation($"Successfully loaded instance for type {typeof(T).Name}, version {_currentVersion}.");
        return loadedInstance;
    }
    public T CommitReset()
    {
        Instance = new T();
        TimestampCreated = DateTimeOffset.Now;
        TimestampSaved = null;
        SaveChanges();
        return Instance;
    }

    public void SaveChanges()
    {
        var filePath = GetStorageFilePath();
        if (Path.GetDirectoryName(filePath) is { } dir)
            Directory.CreateDirectory(dir);

        // Serialize Instance to a JsonNode and add metadata
        Instance.OnSaving(); // Allow the instance to perform pre-save logic
        var node = JsonSerializer.SerializeToNode(Instance, _settings.JsonSerializerOptions);
        if (node is JsonObject jsonObject)
        {
            TimestampSaved = DateTimeOffset.Now;
            jsonObject.Add("_version", _currentVersion);
            jsonObject.Add("_timestampCreated", TimestampCreated);
            jsonObject.Add("_timestampSaved", TimestampSaved);
        }

        // Serialize the modified JsonNode to string
        var jsonData = node?.ToJsonString() ?? throw new InvalidOperationException("Failed to serialize or add metadata to JSON node.");

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
        Instance.OnSaved(); // Call OnSaved after successful write
    }

    public void Dispose()
    {
        SaveChanges();
        GC.SuppressFinalize(this);
    }

    private string GetStorageFilePath()
    {
        // 1. Check if the instance itself defines a custom absolute file path
        if (new T().GetCustomFilePath() is { } customPath)
        {
            return customPath;
        }

        // 2. Otherwise, use the StorageDirectory from settings
        var storageDir = _settings.StorageDirectory
                         ?? AppContext.BaseDirectory; // or fallback to app directory
        var fileName = _isUserProtected
            ? $"{typeof(T).Name}.bin"
            : $"{typeof(T).Name}.json";

        return Path.Combine(storageDir, fileName);
    }
}
