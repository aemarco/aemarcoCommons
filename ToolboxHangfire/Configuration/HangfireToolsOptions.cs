namespace aemarcoCommons.ToolboxHangfire.Configuration;

public class HangfireToolsOptions
{
    /// <summary>
    /// Logs the lifecycle of all jobs through <see cref="LogEverythingFilter"/>,
    /// defaults to true
    /// </summary>
    public bool LogJobLifecycle { get; set; } = true;

    /// <summary>
    /// Lets all registered <see cref="IUpdateHangfireRecurringSchedule"/> update their schedule on startup,
    /// defaults to false
    /// </summary>
    public bool UpdateRecurringSchedule { get; set; }
}
