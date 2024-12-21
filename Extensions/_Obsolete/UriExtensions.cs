using System;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.Extensions.NetworkExtensions
{
    public static partial class UriExtensions
    {
        [Obsolete("Use TcpPing instead.")]
        public static bool Ping(this Uri uri) => uri.TcpPing();
    }
}
