using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Attributes
{
    public sealed class VersionAttribute : Attribute
    {
        public VersionAttribute(string version)
        {
            Version = Version.Parse(version);
        }
        public Version Version { get; }
    }
}
