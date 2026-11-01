using Hangfire.Client;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using Microsoft.Extensions.Logging;
using Serilog.Context;

#pragma warning disable IDE0130

namespace aemarcoCommons.ToolboxHangfire;


public class LogEverythingFilter :
    IClientFilter,
    IElectStateFilter,
    IApplyStateFilter,
    IServerFilter

{

    private readonly ILogger<LogEverythingFilter> _logger;
    public LogEverythingFilter(
        ILogger<LogEverythingFilter> logger)
    {
        _logger = logger;
    }

    // IClientFilter
    public void OnCreating(CreatingContext context)
    {
        _logger.LogDebug(
            "Creating job with '{jobType}:{jobMethod}:{jobParameters}'",
            context.Job.Type.Name,
            context.Job.Method.Name,
            context.Job.Args);
    }
    public void OnCreated(CreatedContext context)
    {
        _logger.LogInformation(
            "Created Job '{jobId}' with '{jobType}:{jobMethod}:{jobParameters}'",
            context.BackgroundJob?.Id,
            context.Job.Type.Name,
            context.Job.Method.Name,
            context.Job.Args);
    }

    // IElectStateFilter
    public void OnStateElection(ElectStateContext context)
    {
        if (context.CandidateState is FailedState failed)
        {
            _logger.LogWarning(
                failed.Exception,
                "Job '{jobId}' has failed with '{jobType}:{jobMethod}:{jobParameters}'",
                context.BackgroundJob.Id,
                context.BackgroundJob.Job?.Type.Name,
                context.BackgroundJob.Job?.Method.Name,
                context.BackgroundJob.Job?.Args);
        }
    }

    // IApplyStateFilter
    public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
    {
        _logger.LogDebug(
            "Job '{jobId}' state changed from '{oldJobState}' to '{newJobState}' with '{jobType}:{jobMethod}:{jobParameters}'",
            context.BackgroundJob.Id,
            context.OldStateName,
            context.NewState.Name,
            context.BackgroundJob.Job?.Type.Name,
            context.BackgroundJob.Job?.Method.Name,
            context.BackgroundJob.Job?.Args);
    }
    public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
    {
        _logger.LogDebug(
            "Job '{jobId}' state '{oldJobState}' was un-applied.",
            context.BackgroundJob.Id,
            context.OldStateName);
    }

    // IServerFilter
    public void OnPerforming(PerformingContext context)
    {
        var logScope = LogContext.PushProperty(
            "jobId",
            context.BackgroundJob.Id);
        context.Items["JobIdLogProperty"] = logScope;

        _logger.LogDebug(
            "Job '{jobId}' starting to perform with '{jobType}:{jobMethod}:{jobParameters}'",
            context.BackgroundJob.Id,
            context.BackgroundJob.Job?.Type.Name,
            context.BackgroundJob.Job?.Method.Name,
            context.BackgroundJob.Job?.Args);
    }
    public void OnPerformed(PerformedContext context)
    {
        _logger.LogInformation(
            "Job '{jobId}' has been performed with '{jobType}:{jobMethod}:{jobParameters}'",
            context.BackgroundJob.Id,
            context.BackgroundJob.Job?.Type.Name,
            context.BackgroundJob.Job?.Method.Name,
            context.BackgroundJob.Job?.Args);

        if (context.Items.TryGetValue("JobIdLogProperty", out var obj) &&
            obj is IDisposable propScope)
            propScope.Dispose();

    }

}