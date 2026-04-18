namespace ToolboxConsoleManualTests;

internal static class Program
{
    private static async Task Main()
    {
        var done = false;
        while (!done)
        {
            Console.Clear();
            var items = new List<ConsoleMenuItem>();

            // Commands
            items.Add(new ConsoleMenuItem<string>(nameof(ClearLine), ClearLine));
            if (OperatingSystem.IsWindows())
                items.Add(new ConsoleMenuItem<string>(nameof(Hiding), Hiding));

            // Inputs
            items.Add(new ConsoleMenuItem<string>(nameof(Decision), Decision));
            items.Add(new ConsoleMenuItem<string>(nameof(Numbers), Numbers));
            items.Add(new ConsoleMenuItem<string>(nameof(NullableNumbers), NullableNumbers));
            items.Add(new ConsoleMenuItem<string>(nameof(Text), Text));

            // PathNavigation
            items.Add(new ConsoleMenuItem<string>(nameof(PathNavigation), PathNavigation));
            if (OperatingSystem.IsWindows())
                items.Add(new ConsoleMenuItem<string>(nameof(NetworkNavigation), NetworkNavigation));

            // Progress
            items.Add(new ConsoleMenuItem<string>(nameof(SpectreProgress), SpectreProgress));
            items.Add(new ConsoleMenuItem<string>(nameof(SpectreMultiProgress), SpectreMultiProgress));
            items.Add(new ConsoleMenuItem<string>(nameof(SpectreStatus), SpectreStatus));

            // Selection
            items.Add(new ConsoleMenuItem<string>(nameof(Selection), Selection));

            // Exit
            items.Add(new ConsoleMenuSeparator());
            items.Add(new ConsoleMenuItem<string>("Exit", _ => { done = true; }));

            var menu = new ConsoleMenu("Test ToolboxConsole stuff", items);
            await menu.RunConsoleMenuAsync();
        }
    }

    // Commands
    private static async Task ClearLine(string? _)
    {
        Console.Clear();
        var words = new[] { "Some", " text", " should", " appear", " and", " disappear", " like", " magic." };
        foreach (var word in words)
        {
            Console.Write(word);
            await Task.Delay(1000);
        }
        PowerConsole.ClearCurrentLine();
        Describe("See only this... nice!!!");
    }
    private static async Task Hiding(string? _)
    {
        if (!OperatingSystem.IsWindows()) return;

        Describe("Console should hide for 2 seconds");

        PowerConsole.HideWindow();
        await Task.Delay(2000);
        PowerConsole.ShowWindow();
    }


    // Inputs
    private static void Decision(string? _)
    {
        var trueResult = PowerConsole.EnsureDecision("Select Yes here.");
        if (!trueResult)
        {
            Describe("Test failed");
            return;
        }
        var falseResult = PowerConsole.EnsureDecision("Select No here.");
        if (falseResult)
        {
            Describe("Test failed");
            return;
        }
        Describe("Test successful");
    }
    private static void Numbers(string? _)
    {
        Console.WriteLine();
        var answer = PowerConsole.EnsureIntInput("Input 42 here");
        if (answer != 42)
        {
            Describe("Test failed");
            return;
        }
        var seven = PowerConsole.EnsureIntInput("Numbers 5 - 10 are working. Try outside outside as well, but use 7 finally", 5, 10);
        if (seven != 7)
        {
            Describe("Test failed");
            return;
        }

        Console.WriteLine();
        Describe("Test successful", false);
    }
    private static void NullableNumbers(string? _)
    {
        Console.WriteLine();

        //number
        var five = PowerConsole.EnsureNullableIntInput("Input 5 here");
        if (five != 5)
        {
            Describe("Test failed");
            return;
        }

        //null
        var notANumber = PowerConsole.EnsureNullableIntInput("Fail with input 'aaa' here, then input nothing");
        if (notANumber is not null)
        {
            Describe("Test failed");
            return;
        }

        //range
        var eight = PowerConsole.EnsureNullableIntInput("Input 8 here", 5, 8);
        if (eight != 8)
        {
            Describe("Test failed");
            return;
        }

        //outside range
        var six = PowerConsole.EnsureNullableIntInput("Fail with 9 first, then use 6", 5, 8);
        if (six != 6)
        {
            Describe("Test failed");
            return;
        }

        //null
        var notANumber2 = PowerConsole.EnsureNullableIntInput("Fail with 'aaa' here, then input nothing", 5, 10);
        if (notANumber2 is not null)
        {
            Describe("Test failed");
            return;
        }

        Console.WriteLine();
        Describe("Test successful", false);
    }
    private static void Text(string? _)
    {
        Console.WriteLine();

        var aaa = PowerConsole.EnsureTextInput("Input 'aaa' here");
        if (aaa != "aaa")
        {
            Describe("Test failed");
            return;
        }

        var aa = PowerConsole.EnsureTextInput("Fail with 'a' here, then input 'aa'", 2);
        if (aa != "aa")
        {
            Describe("Test failed");
            return;
        }

        Console.WriteLine();
        Describe("Test successful", false);
    }


    // PathNavigation
    private static void PathNavigation(string? _)
    {
        var currentPath = Environment.CurrentDirectory;

        Describe("Navigate 2 folders up");
        var twoFoldersUp = PowerConsole.PathSelector(currentPath);
        if (twoFoldersUp != new DirectoryInfo(currentPath).Parent!.Parent!.FullName)
        {
            Describe("Test failed");
            return;
        }

        Describe("Navigate to the root. Use Drive navigation also");
        var root = PowerConsole.PathSelector(currentPath);
        var rootDir = new DirectoryInfo(currentPath).Root;
        if (root != rootDir.FullName)
        {
            Describe("Test failed");
            return;
        }

        Describe("Test successful");
    }
    private static void NetworkNavigation(string? _)
    {
        Console.WriteLine();
        var share = PowerConsole.EnsureTextInput("Input server name here", 3).ToUpper();
        var root = new DirectoryInfo(Environment.CurrentDirectory).Root;
        Describe(@"Navigate to any share on given server");
        var home = PowerConsole.PathSelector(root.FullName, new[] { share });
        if (!home.StartsWith($"\\\\{share}\\"))
        {
            Describe("Test failed");
            return;
        }

        Describe("Test successful");
    }


    // Progress
    private static async Task SpectreProgress(string? _)
    {
        Console.Clear();
        await PowerConsole.StartProgressAsync(async ctx =>
        {
            var task = ctx.AddTask("Processing items", maxValue: 25);
            for (var i = 0; i < 25; i++)
            {
                await Task.Delay(150);
                task.Increment(1);
            }
        });
        Describe("Test successful if progress bar reached 100%", false);
    }
    private static async Task SpectreMultiProgress(string? _)
    {
        Console.Clear();
        await PowerConsole.StartProgressAsync(async ctx =>
        {
            var taskA = ctx.AddTask("Task A", maxValue: 20);
            var taskB = ctx.AddTask("Task B", maxValue: 10);
            for (var i = 0; i < 20; i++)
            {
                await Task.Delay(100);
                taskA.Increment(1);
                if (i % 2 == 0)
                    taskB.Increment(1);
            }
        });
        Describe("Test successful if both progress bars completed", false);
    }
    private static async Task SpectreStatus(string? _)
    {
        Console.Clear();
        await PowerConsole.StartStatusAsync(async _ =>
        {
            await Task.Delay(3000);
            return true;
        });
        Describe("Test successful if you saw a spinner for ~3 seconds", false);
    }


    // Selection
    private static void Selection(string? _)
    {
        var persons = new[]
        {
            "Bob",
            "Alice",
            "Tim"
        };

        var alice = PowerConsole.EnsureSelection("Select Alice here", persons, x => x);
        if (alice != "Alice")
        {
            Describe("Test failed");
            return;
        }

        var tim = PowerConsole.AbortableSelection("Select Tim here", persons, x => x);
        if (tim != "Tim")
        {
            Describe("Test failed");
            return;
        }

        var nobody = PowerConsole.AbortableSelection("Select Abort here", persons, x => x);
        if (nobody is not null)
        {
            Describe("Test failed");
            return;
        }

        Describe("Test successful");
    }




    // helpers
    private static void Describe(string message, bool clear = true)
    {
        if (clear) Console.Clear();
        Console.Write(message);
        Console.Write(". \tPress any key to proceed");
        Console.ReadKey();
    }
}
