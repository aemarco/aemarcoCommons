using System;
using System.Net.Sockets;

namespace Extensions.netExtensions
{
    public static class UriExtensions
    {

        public static bool Ping(this Uri uri)
        {
            TcpClient client = null;
            try
            {
                client = new TcpClient(uri.Host, uri.Port);
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
            finally
            {
                client?.Dispose();
            }
        }



    }
}
