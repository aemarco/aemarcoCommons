using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.Extensions.NetworkExtensions;

public static class UriExtensions
{

    public static bool TcpPing(this Uri uri)
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



    private static readonly HttpClient HttpClient = new();
    public static bool ReturnsOkay(this Uri uri)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Head, uri);
            using var response = HttpClient.Send(request);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }

    public static async Task<bool> ReturnsOkayAsync(this Uri uri, CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Head, uri);
            using var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
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
            // because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(
                    new ProcessStartInfo(
                        "cmd",
                        $"/c start {uri.AbsoluteUri.Replace("&", "^&")}")
                    {
                        CreateNoWindow = true
                    });
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