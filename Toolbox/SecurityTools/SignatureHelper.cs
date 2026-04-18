using System;
using System.Security.Cryptography;
using System.Text;

namespace aemarcoCommons.Toolbox.SecurityTools;

public static class SignatureHelper
{
    public static string CreateSignature(string data, string secret)
    {
        ArgumentException.ThrowIfNullOrEmpty(data);
        ArgumentException.ThrowIfNullOrEmpty(secret);

        if (Encoding.UTF8.GetByteCount(secret) < 32)
            throw new ArgumentException("Secret must be at least 32 bytes.", nameof(secret));

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

}
