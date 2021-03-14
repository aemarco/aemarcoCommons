using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace aemarcoCommons.WebTools.Extensions
{
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
        
        public static string GetAbsolutePath(this HttpContext context)
        {
            var uri = $"{context.GetBasePath()}{context.Request.Path.ToUriComponent()}{context.Request.QueryString.ToUriComponent()}";
            return uri;
        }

        public static string GetAccessToken(this HttpContext context)
        {
            var token = context?.Request.Headers[HeaderNames.Authorization].ToString();
            return token;
        }
    }
}
