// ReSharper disable CheckNamespace
#pragma warning disable IDE0130

namespace aemarcoCommons.ToolboxTypeStore;

public interface ITypeToFileValue
{
    /// <summary>
    /// Gets a custom absolute file path where this instance should be stored.
    /// Return null for default store logic.
    /// </summary>
    string? GetCustomFilePath() => null;

    /// <summary>
    /// Called by the store just before the instance is saved.
    /// Use this to perform any pre-save logic.
    /// </summary>
    void OnSaving() { }

    /// <summary>
    /// Called by the store just after the instance has been successfully saved to disk.
    /// Use this to perform any post-save logic.
    /// </summary>
    void OnSaved() { }
}
