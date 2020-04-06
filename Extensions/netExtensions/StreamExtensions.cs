using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Extensions.netExtensions
{
    public static class StreamExtensions
    {
        public static string Hash(this Stream stream)
        {
            using var md5 = MD5.Create();
            return Convert.ToBase64String(md5.ComputeHash(stream));
        }
    }
}
