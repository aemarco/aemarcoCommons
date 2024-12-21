using aemarcoCommons.Extensions.NetworkExtensions;
using System.Net;
using System.Threading.Tasks;

namespace aemarcoCommons.Toolbox.NetworkTools
{
    public static class Internet
    {
        /// <summary>
        /// Checks if the internet is accessible by pinging a default external host.
        /// </summary>
        /// <param name="timeout">The timeout in milliseconds for the ping operation.</param>
        /// <returns>True if the internet is accessible; otherwise, false.</returns>
        public static async Task<bool> IsAccessible(int timeout = 3000)
        {
            var ip = IPAddress.Parse("8.8.8.8"); // Default to Google's DNS
            return await ip.IsPingable(timeout);
        }
    }
}
