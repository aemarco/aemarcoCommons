using System;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.Toolbox.SerializationTools
{

    [Obsolete("Use TypeofToFileValue instead.")]
    public abstract class BaseTypeToFileValue : ITypeToFileValue
    {
        public abstract string Filepath { get; }
        public virtual int CurrentVersion => 0;

        public int Version { get; set; }
        public DateTimeOffset TimestampCreated { get; set; }
        public DateTimeOffset TimestampSaved { get; set; }
    }
}
