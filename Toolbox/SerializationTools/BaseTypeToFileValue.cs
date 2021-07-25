using System;

namespace aemarcoCommons.Toolbox.SerializationTools
{
    public abstract class BaseTypeToFileValue : ITypeToFileValue
    {
        public abstract string Filepath { get; }
        public virtual int CurrentVersion => 0;

        public int Version { get; set; }
        public DateTimeOffset TimestampCreated { get; set; }
        public DateTimeOffset TimestampSaved { get; set; }
    }
}