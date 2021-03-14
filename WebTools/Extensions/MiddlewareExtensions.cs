using System;
using aemarcoCommons.WebTools.Exceptions;
using aemarcoCommons.WebTools.Middleware;
using Microsoft.AspNetCore.Builder;

namespace aemarcoCommons.WebTools.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void UseExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware<Exception>>();
            app.UseMiddleware<ExceptionMiddleware<BadRequestException>>();
        }

        public static void UseBearerToHeaderMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<BearerToHeaderMiddleware>();
        } 

    }
}
