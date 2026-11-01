namespace aemarcoCommons.ToolboxWeb.Models;

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