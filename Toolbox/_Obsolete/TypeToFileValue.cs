using Newtonsoft.Json;
using System;

#pragma warning disable IDE0130
namespace aemarcoCommons.Toolbox.SerializationTools;
#pragma warning restore IDE0130

[Obsolete("Use ToolboxTypeStore instead.")]
[AttributeUsage(AttributeTargets.Class)]
public class UserProtectedAttribute : Attribute
{

}

[Obsolete("Use ToolboxTypeStore instead.")]
public interface ITypeToFileValue
{
    /// <summary>
    /// set this to null when configured through IJsonTypeToFileSettings
    /// </summary>
    [JsonIgnore]
    string Filepath { get; }
    /// <summary>
    /// set this to 0 when versioning isn´t used.
    /// upping the version will ignore loaded file
    /// </summary>
    [JsonIgnore]
    int CurrentVersion { get; }


    int Version { get; set; }
    DateTimeOffset TimestampCreated { get; set; }
    DateTimeOffset TimestampSaved { get; set; }
}

[Obsolete("Use ToolboxTypeStore instead.")]
public abstract class TypeToFileValue : ITypeToFileValue
{
    public virtual string Filepath { get; } = null;
    public virtual int CurrentVersion => 0;

    public int Version { get; set; }
    public DateTimeOffset TimestampCreated { get; set; }
    public DateTimeOffset TimestampSaved { get; set; }
}