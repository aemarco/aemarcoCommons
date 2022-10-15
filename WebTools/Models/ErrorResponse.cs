// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace aemarcoCommons.WebTools.Models;

public class ErrorResponse
{
    public ErrorResponse(int statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message;
    }

    /// <summary>
    /// Http Status Code
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// some message
    /// </summary>
    public string Message { get; }
}