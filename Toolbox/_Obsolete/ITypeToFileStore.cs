using System;

#pragma warning disable IDE0130
namespace aemarcoCommons.Toolbox.SerializationTools;
#pragma warning restore IDE0130

[Obsolete("Use ToolboxTypeStore instead.")]
public interface ITypeToFileStore<out T> : IDisposable
    where T : class, ITypeToFileValue, new()
{
    T Instance { get; }
    T CommitReset();
    void SaveChanges();
}