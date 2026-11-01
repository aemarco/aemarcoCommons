using Hangfire;
using Hangfire.Common;
using Hangfire.States;

#pragma warning disable IDE0130

namespace aemarcoCommons.ToolboxHangfire;

public static class BackgroundJobClientExtensions
{
    /// <summary>
    /// Creates an already succeeded job, which can be used as parent for continuations
    /// </summary>
    /// <returns>Id of the created job</returns>
    public static string GetDummyStarter(this IBackgroundJobClient client)
    {
        return client.Create(
            Job.FromExpression(() => new NoOp().Starter()),
            new SucceededState(null, 0, 0));
    }
}

public class NoOp
{
    public Task Starter()
    {
        return Task.CompletedTask;
    }
}
