// ReSharper disable CheckNamespace
#pragma warning disable IDE0130

namespace aemarcoCommons.ToolboxTypeStore;

public interface ITypeToFileValue
{
    /// <summary>
    /// Gets or sets the timestamp, the instance was created
    /// </summary>
    DateTimeOffset TimestampCreated { get; set; }

    /// <summary>
    /// Gets or sets the timestamp, the instance was saved
    /// </summary>
    DateTimeOffset? TimestampSaved { get; set; }

    /// <summary>
    /// Gets or sets the version of the data
    /// </summary>
    int Version { get; set; }

    /// <summary>
    /// Gets the current version of the data structure
    /// </summary>
    int CurrentVersion { get; }

    /// <summary>
    /// Gets the path where this file should be stored.
    /// Default is null, so store decides based on its settings
    /// </summary>
    string? Filepath { get; }
}
