using aemarcoCommons.ConsoleTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace ConsoleToolsManualTests;



internal static class Program
{
    #region menu

    private static Task Main()
    {
        var done = false;
        while (!done)
        {
            Console.Clear();
            var items = new List<ConsoleMenuItem>();
            AddMenuItems(items);
            //Exit
            items.AddRange(new List<ConsoleMenuItem>
            {
                //Exit
                new ConsoleMenuSeparator(),
                new ConsoleMenuItem<string>("Exit", _ =>
                {
                    done = true;
                })
            });
            var menu = new ConsoleMenu("Test ConsoleTools stuff", items);
            menu.RunConsoleMenu();
        }

        return Task.CompletedTask;
    }


    #endregion

    #region Testhelpers

    private static void Describe(string message, bool clear = true)
    {
        if (clear) Console.Clear();
        Console.Write(message);
        Console.Write(". \tPress any key to proceed");
        Console.ReadKey();
    }

    #endregion

    private static void AddMenuItems(ICollection<ConsoleMenuItem> items)
    {
        AddPositionTests(items);
        AddWindowTests(items);
        AddSelectionTests(items);
        AddDecisionTests(items);
        AddNumbersTests(items);
        AddNullableNumbersTests(items);
        AddTextTests(items);
        AddPathNavigationTests(items);
        AddNetworkNavigationTests(items);
        AddProgressTests(items);
    }



    private static void AddWindowTests(ICollection<ConsoleMenuItem> items)
    {
        if (OperatingSystem.IsWindows())
        {
            items.Add(new ConsoleMenuItem<string>("Hiding", async _ =>
            {
                if (!OperatingSystem.IsWindows()) return;

                Describe("Console should hide for 2 seconds");

                PowerConsole.HideWindow();
                await Task.Delay(2000);
                PowerConsole.ShowWindow();
            }));
        }
    }

    private static void AddPositionTests(ICollection<ConsoleMenuItem> items)
    {
        items.Add(new ConsoleMenuItem<string>("ClearLine", async _ =>
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
        }));
    }

    private static void AddSelectionTests(ICollection<ConsoleMenuItem> items)
    {
        items.Add(new ConsoleMenuItem<string>("Selection", _ =>
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

        }));
    }

    private static void AddDecisionTests(ICollection<ConsoleMenuItem> items)
    {
        items.Add(new ConsoleMenuItem<string>("Decision", _ =>
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

        }));
    }

    private static void AddNumbersTests(ICollection<ConsoleMenuItem> items)
    {
        items.Add(new ConsoleMenuItem<string>("Numbers", _ =>
        {
            Console.WriteLine();
            var answer = PowerConsole.EnsureIntInput("Input 42 here");
            if (answer != 42)
            {
                Describe("Test failed");
                return;
            }
            var seven = PowerConsole.EnsureIntInputInRange("Numbers 5 - 10 are working. Try outside outside as well, but use 7 finally", 5, 10);
            if (seven != 7)
            {
                Describe("Test failed");
                return;
            }

            Console.WriteLine();
            Describe("Test successful", false);

        }));
    }

    private static void AddNullableNumbersTests(ICollection<ConsoleMenuItem> items)
    {
        items.Add(new ConsoleMenuItem<string>("NullableNumbers", _ =>
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
            var eight = PowerConsole.EnsureNullableIntInputInRange("Input 8 here", 5, 8);
            if (eight != 8)
            {
                Describe("Test failed");
                return;
            }

            //outside range
            var six = PowerConsole.EnsureNullableIntInputInRange("Fail with 9 first, then use 6", 5, 8);
            if (six != 6)
            {
                Describe("Test failed");
                return;
            }

            //null
            var notANumber2 = PowerConsole.EnsureNullableIntInputInRange("Fail with 'aaa' here, then input nothing", 5, 10);
            if (notANumber2 is not null)
            {
                Describe("Test failed");
                return;
            }

            Console.WriteLine();
            Describe("Test successful", false);

        }));
    }

    private static void AddTextTests(ICollection<ConsoleMenuItem> items)
    {
        items.Add(new ConsoleMenuItem<string>("Text", _ =>
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

        }));
    }

    private static void AddPathNavigationTests(ICollection<ConsoleMenuItem> items)
    {
        items.Add(new ConsoleMenuItem<string>("PathNavigation", _ =>
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
        }));
    }

    private static void AddNetworkNavigationTests(ICollection<ConsoleMenuItem> items)
    {
        if (OperatingSystem.IsWindows())
        {
            items.Add(new ConsoleMenuItem<string>("NetworkNavigation", _ =>
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
            }));
        }
    }

    private static void AddProgressTests(ICollection<ConsoleMenuItem> items)
    {
        items.Add(new ConsoleMenuItem<string>("ConsoleTopProgress", async _ =>
        {
            Console.WriteLine();

            // ReSharper disable StringLiteralTypo
            var text = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Duis autem vel eum iriure";
            // ReSharper restore StringLiteralTypo

            var progress = new ConsoleTopProgressBar(4);
            for (var i = 0; i < 25; i++)
            {
                for (var y = 0; y < 4; y++)
                {
                    var line = text[..(text.IndexOf(' ') + 1)];
                    if (text.Length > line.Length) text = text[line.Length..];
                    Console.WriteLine(line);
                }

                await Task.Delay(150);
                progress.UpdateProgress(i + 1, 25);
            }
            Describe("Test successful if you always saw the progress bar, and it reached 100%", false);
        }));


        items.Add(new ConsoleMenuItem<string>("ConsoleInlineProgress", async _ =>
        {
            Console.WriteLine();

            var progress = new ConsoleInlineProgressBar();

            for (var i = 0; i < 25; i++)
            {
                await Task.Delay(150);
                progress.UpdateProgress(i + 1, 25);
            }
            Describe("Test successful if you see progress to 100% in multiple lines", false);
        }));


        items.Add(new ConsoleMenuItem<string>("ConsoleOneLineProgress", async _ =>
        {
            Console.WriteLine();

            var progress = new ConsoleOneLineProgressBar();

            for (var i = 0; i < 25; i++)
            {
                await Task.Delay(150);
                progress.UpdateProgress(i + 1, 25);
            }
            Describe("Test successful if you see progress to 100% in one line", false);
        }));


    }


}