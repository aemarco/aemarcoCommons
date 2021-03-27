using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace aemarcoCommons.Extensions.NetworkExtensions
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

        public static void OpenInBrowser(this Uri uri)
        {
            try
            {
                Process.Start(uri.AbsoluteUri);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var url = uri.AbsoluteUri.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", uri.AbsoluteUri);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", uri.AbsoluteUri);
                }
                else
                {
                    throw;
                }
            }
        }


    }
}
