using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools;

public static partial class PowerConsole
{

    public static void StartProgress(Action<ProgressContext> work)
    {
        AnsiConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn())
            .Start(work);
    }

    public static T StartProgress<T>(Func<ProgressContext, T> work)
    {
        return AnsiConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn())
            .Start(work);
    }


    public static async Task StartProgressAsync(Func<ProgressContext, Task> work)
    {
        await AnsiConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn())
            .StartAsync(work);
    }

    public static async Task StartProgressAsync<T>(
        Func<ProgressContext, T, CancellationToken, Task> work,
        T request,
        CancellationToken cancellationToken)
    {
        await StartProgressAsync(async ctx =>
            await work(ctx, request, cancellationToken));
    }


    public static async Task<T> StartProgressAsync<T>(Func<ProgressContext, Task<T>> work)
    {
        return await AnsiConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn())
            .StartAsync(work);
    }

}