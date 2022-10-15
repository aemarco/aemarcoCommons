using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// ReSharper disable ClassNeverInstantiated.Global

namespace aemarcoCommons.WebTools.Middleware;

public static class BearerToHeaderMiddlewareExtensions
{

    public static void UseBearerToHeaderMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<BearerToHeaderMiddleware>();
    }

}

/// <summary>
/// This Middleware takes access tokens from query parameter, removes it there and puts in as authorization header
/// https://tools.ietf.org/html/rfc6750#section-2.3
/// </summary>
public class BearerToHeaderMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<BearerToHeaderMiddleware> _logger;

    public BearerToHeaderMiddleware(
        RequestDelegate next,
        ILogger<BearerToHeaderMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Query.TryGetValue("access_token", out var token))
        {
            var items = context.Request.Query
                .SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value))
                .ToList();
            items.RemoveAll(x => x.Key.Equals("access_token", StringComparison.OrdinalIgnoreCase));
            context.Request.QueryString = new QueryBuilder(items).ToQueryString();
            context.Request.Headers.Add("Authorization", $"Bearer {token}");

            _logger.LogDebug("Access token moved to Authorization header");
        }
        await _next(context);



    }
}