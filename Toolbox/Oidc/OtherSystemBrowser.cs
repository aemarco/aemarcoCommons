using aemarcoCommons.Extensions.NetworkExtensions;
using IdentityModel.OidcClient.Browser;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.Toolbox.Oidc
{
    public class OtherSystemBrowser : IBrowser
    {
        private readonly int _port;
        private readonly string _postLoginUrl;
        public OtherSystemBrowser(int port, string postLoginUrl)
        {
            //_port = GetRandomUnusedPort();
            _port = port;
            _postLoginUrl = postLoginUrl;
        }

        //need a flexible allow redirect url at id server first
        public string RedirectUri => $"http://{IPAddress.Loopback}:{_port}/";

        //private static int GetRandomUnusedPort()
        //{
        //    var listener = new TcpListener(IPAddress.Loopback, 0);
        //    listener.Start();
        //    var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        //    listener.Stop();
        //    return port;
        //}

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add(RedirectUri);
                listener.Start();

                new Uri(options.StartUrl).OpenInBrowser();
                try
                {
                    var ctx = await listener.GetContextAsync();
                    using (var resp = ctx.Response)
                    {
                        resp.StatusCode = (int)HttpStatusCode.OK;
                        resp.StatusDescription = "Status OK";

                        var query = ctx.Request.Url?.Query;
                        var html = GetHtml(query);

                        var buffer = Encoding.Default.GetBytes(html);
                        await resp.OutputStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
                        await resp.OutputStream.FlushAsync(cancellationToken);

                        //no understand why it does not work without delay ?! flush also did not help...
                        await Task.Delay(500, cancellationToken);

                        var result = new BrowserResult
                        {
                            Response = query,
                            ResultType = BrowserResultType.Success
                        };
                        return result;
                    }
                }
                catch (TaskCanceledException ex)
                {
                    return new BrowserResult
                    {
                        ResultType = BrowserResultType.Timeout,
                        Error = ex.Message
                    };
                }
                catch (Exception ex)
                {
                    return new BrowserResult
                    {
                        ResultType = BrowserResultType.UnknownError,
                        Error = ex.Message
                    };
                }
                finally
                {
                    listener.Stop();
                }
            }
        }

        private string GetHtml(string query)
        {
            string message, template;
            if (string.IsNullOrWhiteSpace(query))
            {
                message = "Logout success";
                template = AutoClose;
            }
            else if (query.Contains("error=access_denied"))
            {
                message = "Login aborted";
                template = AutoClose;
            }
            else
            {
                message = "Login success";
                template = Redirect;
            }

            var content = template
                .Replace("{{{message}}}", $"<h1>{message}</h1>")
                .Replace("{{{postLogoutUrl}}}", _postLoginUrl);

            return content;
        }

        private const string AutoClose = "<!DOCTYPE html><html><body>{{{message}}}<script>window.addEventListener('load',function(){close();});</script></body></html>";
        private const string Redirect = "<!DOCTYPE html><html><body>{{{message}}}<script>window.addEventListener('load',function(){window.location.assign('{{{postLogoutUrl}}}');});</script></body></html>";

    }
}
