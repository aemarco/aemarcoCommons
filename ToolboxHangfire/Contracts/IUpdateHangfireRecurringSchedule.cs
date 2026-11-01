using Hangfire;

#pragma warning disable IDE0130

namespace aemarcoCommons.ToolboxHangfire;

/// <summary>
/// Implemented by modules which own recurring jobs, to register their schedule on startup
/// </summary>
public interface IUpdateHangfireRecurringSchedule
{
    public void UpdateRecurringSchedule(IRecurringJobManager manager);
}
