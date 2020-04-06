using System;
using System.Net;
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


        public static bool ReturnsOkay(this Uri uri)
        {
            HttpWebResponse response = null;
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "HEAD";

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException)
            {
                /* A WebException will be thrown if the status of the response is not `200 OK` */
                return false;
            }
            finally
            {
                // Don't forget to close your response.
                response?.Close();
            }
        }


    }
}
