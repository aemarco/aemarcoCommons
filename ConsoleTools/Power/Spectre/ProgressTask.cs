// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools;

public static partial class PowerConsole
{

    public static void CompleteTask(this ProgressTask task)
    {
        task.Value = task.MaxValue;
        task.IsIndeterminate = false;
        task.StopTask();
    }

}

