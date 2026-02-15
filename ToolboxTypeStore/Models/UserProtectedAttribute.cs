#pragma warning disable IDE0130

namespace aemarcoCommons.ToolboxTypeStore;

/// <summary>
/// When marked with this attribute, the file will be user-specific-encrypted
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class UserProtectedAttribute : Attribute;
