using aemarcoCommons.WebTools.Exceptions;
using aemarcoCommons.WebTools.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
// ReSharper disable ClassNeverInstantiated.Global

namespace aemarcoCommons.WebTools.Middleware;

public static class ExceptionMiddlewareExtensions
{
    public static void UseExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware<Exception>>();
        app.UseMiddleware<ExceptionMiddleware<BadRequestException>>();
    }
    public static void UseDeveloperExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseDeveloperExceptionPage();
        app.UseMiddleware<ExceptionMiddleware<BadRequestException>>();
    }
}

public class ExceptionMiddleware<T> where T : Exception
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware<T>> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware<T>> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (T ex)
        {
            await HandleException(context, ex);
        }
    }


    private Task HandleException(HttpContext context, T ex)
    {
        ErrorResponse errorResponse;
        if (ex is BadRequestException badRequestException)
        {
            errorResponse = new ErrorResponse((int)HttpStatusCode.BadRequest, badRequestException.Message);
            _logger.LogWarning(badRequestException, "BadRequest response {@errorResponse}", errorResponse);
        }
        else //deal with unhandled exceptions
        {
            errorResponse = new ErrorResponse((int)HttpStatusCode.InternalServerError, "Upsss, something went wrong");
            _logger.LogError(ex, "Error response {@errorResponse}", errorResponse);
        }


        //send response
        context.Response.StatusCode = errorResponse.StatusCode;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }

}