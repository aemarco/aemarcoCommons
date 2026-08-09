using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;

namespace aemarcoCommons.ToolboxWeb.Extensions;

public static class ContextExtensions
{
    public static string GetRootPath(this HttpContext context)
    {
        var uri = $"{context.Request.Scheme}://{context.Request.Host.ToUriComponent()}";
        return uri;
    }

    public static string GetBasePath(this HttpContext context)
    {
        var uri = $"{context.GetRootPath()}{context.Request.PathBase.ToUriComponent()}";
        return uri;
    }

    public static string GetAbsolutePath(this HttpContext context) =>
        context.Request.GetEncodedUrl();

    public static string? GetAccessToken(this HttpContext context)
    {
        var header = context.Request.Headers[HeaderNames.Authorization].ToString();
        return header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? header["Bearer ".Length..]
            : null;
    }
}