using Hangfire;
using Hangfire.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace aemarcoCommons.ToolboxHangfire.Services;

/// <summary>
/// On startup, lets all registered <see cref="IUpdateHangfireRecurringSchedule"/> update their schedule,
/// and puts all recurring jobs into the local time zone.
/// Register it only where the schedule is supposed to be updated.
/// </summary>
internal class UpdateHangfireRecurringSchedule : BackgroundService
{

    private readonly IServiceScopeFactory _scopeFactory;
    public UpdateHangfireRecurringSchedule(
        IGlobalConfiguration hangfireGlobalConfiguration, //so that configuration gets executed
        IServiceScopeFactory scopeFactory)
    {
        _ = hangfireGlobalConfiguration;
        _scopeFactory = scopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var recurring = scope.ServiceProvider.GetServices<IUpdateHangfireRecurringSchedule>();
        var recurringManger = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
        foreach (var updater in recurring)
            updater.UpdateRecurringSchedule(recurringManger);

        //update all recurring jobs to be in local time zone
        var storage = scope.ServiceProvider.GetRequiredService<JobStorage>();
        using var connection = storage.GetConnection();
        foreach (var job in connection.GetRecurringJobs())
        {
            if (job.Job is null)
                continue;

            var hash = connection.GetAllEntriesFromHash($"recurring-job:{job.Id}");
            var mode = hash.TryGetValue("Misfire", out var stringVal) &&
                       int.TryParse(stringVal, out var enumVal)
                ? (MisfireHandlingMode)enumVal
                : MisfireHandlingMode.Relaxed;

            recurringManger.AddOrUpdate(
                job.Id,
                job.Job,
                job.Cron,
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    MisfireHandling = mode
                });
        }

        return Task.CompletedTask;
    }

}
