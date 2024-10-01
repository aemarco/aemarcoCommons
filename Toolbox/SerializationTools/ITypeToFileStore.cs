using System;

namespace aemarcoCommons.Toolbox.SerializationTools
{
    public interface ITypeToFileStore<out T> : IDisposable
        where T : class, ITypeToFileValue, new()
    {
        T Instance { get; }
        T CommitReset();
        void SaveChanges();
    }
}