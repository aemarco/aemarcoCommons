using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace aemarcoCommons.Extensions.NetworkExtensions
{
    public static class IpAddressExtensions
    {
        /// <summary>
        /// Checks if a specified Ip is reachable via ping.
        /// </summary>
        /// <param name="ip">The IP address to ping</param>
        /// <param name="timeout">The timeout in milliseconds for the ping operation.</param>
        /// <returns>True if the host is reachable; otherwise, false.</returns>
        public static async Task<bool> IsPingable(this IPAddress ip, int timeout = 3000)
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = await ping.SendPingAsync(ip, timeout);
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }

    }
}
