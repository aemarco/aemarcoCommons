using aemarcoCommons.Extensions.NetworkExtensions;
using IdentityModel.OidcClient.Browser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.Toolbox.Oidc
{
    public class OidcSystemBrowser : IBrowser
    {

        private readonly int _port;
        private readonly string _postLoginUrl;

        public OidcSystemBrowser(int port, string postLoginUrl = null)
        {
            _port = port;
            _postLoginUrl = postLoginUrl;
        }
        //public static int GetRandomUnusedPort()
        //{
        //    var listener = new TcpListener(IPAddress.Loopback, 0);
        //    listener.Start();
        //    var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        //    listener.Stop();
        //    return port;
        //}



        public string RedirectUri => $"http://127.0.0.1:{_port}/";

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken)
        {
            using (var listener = new LoopbackHttpListener(options.EndUrl, _postLoginUrl))
            {
                new Uri(options.StartUrl).OpenInBrowser();
                try
                {
                    var result = await listener.WaitForCallbackAsync(options.Timeout, cancellationToken);

                    if (options.StartUrl.Contains("endsession"))
                    {
                        return string.IsNullOrWhiteSpace(result)
                            ? new BrowserResult { Response = result, ResultType = BrowserResultType.Success }
                            : new BrowserResult { ResultType = BrowserResultType.UnknownError, Error = result };
                    }

                    return string.IsNullOrWhiteSpace(result)
                        ? new BrowserResult { ResultType = BrowserResultType.UnknownError, Error = "Empty response." }
                        : new BrowserResult { Response = result, ResultType = BrowserResultType.Success };
                }
                catch (TaskCanceledException ex)
                {
                    return new BrowserResult
                    {
                        ResultType = cancellationToken.IsCancellationRequested
                            ? BrowserResultType.UserCancel
                            : BrowserResultType.Timeout,
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
                    await listener.CloseListener();
                }
            }
        }
    }

    internal class LoopbackHttpListener : IDisposable
    {
        private readonly IWebHost _host;
        private readonly TaskCompletionSource<string> _source;
        private readonly string _postLoginUrl;
        public LoopbackHttpListener(string url, string postLoginUrl)
        {
            _postLoginUrl = postLoginUrl;
            _source = new TaskCompletionSource<string>();

            _host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(url)
                .Configure(Configure)
                .Build();
            _host.Start();
        }

        private void Configure(IApplicationBuilder app)
        {
            app.Run(async ctx =>
            {
                switch (ctx.Request.Method)
                {
                    case "GET":

                        var message = string.IsNullOrWhiteSpace(ctx.Request.QueryString.Value)
                            ? "Logout success"
                            : ctx.Request.QueryString.Value.Contains("error=access_denied")
                                ? "Login aborted"
                                : "Login success";


                        var template = string.IsNullOrWhiteSpace(ctx.Request.QueryString.Value)
                            ? AutoClose
                            : _postLoginUrl == null
                                ? AutoClose
                                : Redirect;

                        await SetResult(ctx.Request.QueryString.Value, ctx, template, message);
                        break;
                    case "POST" when !ctx.Request.ContentType.Equals("application/x-www-form-urlencoded",
                        StringComparison.OrdinalIgnoreCase):
                        ctx.Response.StatusCode = 415;
                        break;
                    case "POST":
                        {
                            using (var sr = new StreamReader(ctx.Request.Body, Encoding.UTF8))
                            {
                                var body = await sr.ReadToEndAsync();
                                await SetResult(body, ctx, AutoClose, "Success");
                                break;
                            }
                        }
                    default:
                        ctx.Response.StatusCode = 405;
                        break;
                }
            });
        }




        private const string AutoClose = @"<!DOCTYPE html><html><body>{{{message}}}<script>window.addEventListener('load',function(){close();});</script></body></html>";
        private const string Redirect = @"<!DOCTYPE html><html><body>{{{message}}}<script>window.addEventListener('load',function(){window.location.assign('{{{postLogoutUrl}}}');});</script></body></html>";

        private async Task SetResult(
            string value,
            HttpContext ctx,
            string template,
            string message)
        {
            try
            {
                var content = template
                    .Replace("{{{message}}}", $"<h1>{message}</h1>")
                    .Replace("{{{postLogoutUrl}}}", _postLoginUrl);

                ctx.Response.StatusCode = 200;
                ctx.Response.ContentType = "text/html";
                ctx.Response.Headers.Add("Date", DateTimeOffset.UtcNow.ToString());

                await ctx.Response.WriteAsync(content);
                await ctx.Response.Body.FlushAsync();

                _source.TrySetResult(value);
            }
            catch
            {
                ctx.Response.StatusCode = 400;
                ctx.Response.ContentType = "text/html";
                await ctx.Response.WriteAsync("<h1>Invalid request.</h1>");
                await ctx.Response.Body.FlushAsync();
            }
        }

        public Task<string> WaitForCallbackAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(timeout, cancellationToken);
                }
                catch (OperationCanceledException)
                {

                }
                _source.TrySetCanceled(cancellationToken);
            }, cancellationToken);
            return _source.Task;
        }

        public async Task CloseListener()
        {
            await Task.Delay(500);
            await _host.StopAsync();
        }

        public void Dispose()
        {
            Task.Run(async () =>
            {
                await Task.Delay(500);
                _host.Dispose();
            });
        }
    }
}
