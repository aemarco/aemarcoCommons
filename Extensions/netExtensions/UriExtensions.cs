using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

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
