using System;

namespace Contracts.Attributes
{

    [Obsolete]
    public sealed class VersionAttribute : Attribute
    {
        public VersionAttribute(string version)
        {
            Version = Version.Parse(version);
        }
        public Version Version { get; }
    }
}
