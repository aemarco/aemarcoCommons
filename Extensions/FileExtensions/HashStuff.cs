using System.IO;
using System.Net;
using Extensions.netExtensions;

namespace Extensions.FileExtensions
{
    public static class HashStuff
    {

        public static string HashTheThing(this string location)
        {
            using var stream = location.StartsWith("http") 
                ? new WebClient().OpenRead(location) 
                : File.OpenRead(location);
            return stream.ToBase64HashString();
        }



    }
}
