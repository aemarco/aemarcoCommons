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
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse?>> logger)
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


            if (!typeof(TRequest).HasAttribute<NoLogAttribute>())
            {
                if (response is Unit)
                    _logger.LogInformation("Handled {typeName} message {@request}", typeName, request);
                else
                    _logger.LogInformation("Handled {typeName} with {@request} and {@response}", typeName, request,
                        response);
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
