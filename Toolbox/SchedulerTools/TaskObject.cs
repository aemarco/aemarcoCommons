using System;

namespace aemarcoCommons.Toolbox.SchedulerTools
{
    public class TaskObject
    {
        public string Name { get; set; } = "NoName";
        public Action PlannedAction { get; set; } = () => { };
        public DateTimeOffset PlannedFor { get; set; } = DateTimeOffset.Now;
        public SchedulingOptions SchedulingOptions { get; set; } = SchedulingOptions.None;

        

        // RetryOnException
        public int MaxRetries { get; set; } = 1;
        internal int RetryCount { get; set; } = 0;
        // RecurringByInterval
        public TimeSpan Interval { get; set; } = TimeSpan.Zero;
        // RecurringByDynamicInterval
        public DateTimeOffset LastStarted { get; set; } = DateTimeOffset.MinValue;
        public Func<TimeSpan> DynamicInterval { get; set; } = () => { return TimeSpan.Zero; };
        // DelayUntilNextTimeWindow
        public TimeSpan TimeWindowStart = new TimeSpan(0, 0, 0, 0);
        public TimeSpan TimeWindowEnd = new TimeSpan(23, 59, 59, 59);


    }
}
