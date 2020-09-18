using System;

namespace aemarcoCommons.Toolbox.SchedulerTools
{
    [Flags]
    public enum SchedulingOptions
    {
        None = 0,
        /// <summary>
        /// Retries for given Intervall "MaxRetries"
        /// </summary>
        RetryOnException = 1 << 0,
        /// <summary>
        /// Schedules given Task based on "PlannedFor" and "Interval"
        /// </summary>
        RecurringByInterval = 1 << 1,
        /// <summary>
        /// Schedules given Task based on "LastStarted" and "DynamicInterval"
        /// </summary>
        RecurringByDynamicInterval = 1 << 2,
        /// <summary>
        /// Delay given Task until next TimeWindow specified by "TimeWindowStart" and "TimeWindowEnd"
        /// </summary>
        DelayUntilNextTimeWindow = 1 << 3,

    }
}
