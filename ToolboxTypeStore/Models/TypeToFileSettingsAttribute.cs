// ReSharper disable CheckNamespace
#pragma warning disable IDE0130

namespace aemarcoCommons.ToolboxTypeStore;

/// <summary>
/// Defines settings for a type being stored by ITypeToFileStore.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class TypeToFileSettingsAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeToFileSettingsAttribute"/> class.
    /// </summary>
    /// <param name="isUserProtected">If true, the file will be user-specific-encrypted using ProtectedData.</param>
    /// <param name="version">The current version of the data structure. Used by the store for reset logic if file version differs.</param>
    public TypeToFileSettingsAttribute(bool isUserProtected = false, int version = 0)
    {
        IsUserProtected = isUserProtected;
        Version = version;
    }
    public bool IsUserProtected { get; }
    public int Version { get; }
}
