// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ToolboxAppOptions;

/// <summary>
/// Skips registration of IOptions, itself etc...
/// </summary>
[Obsolete("Is already ignored")]
[AttributeUsage(AttributeTargets.Class)]
public class SkipRegistrationAttribute : Attribute;