#pragma warning disable IDE0130
namespace aemarcoCommons.ToolboxMediatR;

public static class LoggingBehaviorExtensions
{
    public static MediatRServiceConfiguration AddLoggingBehavior(this MediatRServiceConfiguration config)
    {
        config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        return config;
    }
}

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse?>
    where TRequest : IRequest<TResponse?>
{

    private readonly ILogger<LoggingBehavior<TRequest, TResponse?>> _logger;
    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse?>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse?> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse?> next,
        CancellationToken cancellationToken)
    {
        var typeName = typeof(TRequest).Name;
        try
        {
            var response = await next(cancellationToken);

            if (typeof(TRequest).HasAttribute<NoLogAttribute>())
                return response;

            var logRequest = !typeof(TRequest).HasAttribute<NoLogRequestAttribute>();
            var logResponse =
                response is not Unit &&
                !typeof(TRequest).HasAttribute<NoLogResponseAttribute>();

            switch (logRequest, logResponse)
            {
                case (true, true):
                    _logger.LogInformation("Handled {typeName} with {@request} and {@response}", typeName, request, response);
                    break;
                case (true, false):
                    _logger.LogInformation("Handled {typeName} with {@request}", typeName, request);
                    break;
                case (false, true):
                    _logger.LogInformation("Handled {typeName} with {@response}", typeName, response);
                    break;
                case (false, false):
                    _logger.LogInformation("Handled {typeName}", typeName);
                    break;
            }
            return response;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle {typeName} Message {@request}", typeName, request);
            throw;
        }
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class NoLogAttribute : Attribute;

[AttributeUsage(AttributeTargets.Class)]
public class NoLogRequestAttribute : Attribute;

[AttributeUsage(AttributeTargets.Class)]
public class NoLogResponseAttribute : Attribute;