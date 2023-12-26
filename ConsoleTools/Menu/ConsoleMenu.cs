using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools;

public class ConsoleMenu
{
    private readonly ConsoleMenuItem[] _menuItems;
    private readonly string _description;
    private int _selectedItemIndex;
    private bool _itemIsSelected;

    public ConsoleMenu(string description, IEnumerable<ConsoleMenuItem> menuItems)
    {
        _menuItems = menuItems.ToArray();
        _description = description;
    }

    public void RunConsoleMenu()
    {
        if (!string.IsNullOrEmpty(_description))
        {
            Console.WriteLine($"{_description}: {Environment.NewLine}");
        }

        StartConsoleDrawInLoopUntilInputIsMade();


        _itemIsSelected = false;
        _menuItems[_selectedItemIndex].Execute();
    }
    public async Task RunConsoleMenuAsync()
    {
        if (!string.IsNullOrEmpty(_description))
        {
            AnsiConsole.MarkupLine(_description);
            Console.WriteLine();
        }

        StartConsoleDrawInLoopUntilInputIsMade();


        _itemIsSelected = false;
        await _menuItems[_selectedItemIndex].ExecuteAsync();
    }


    private void StartConsoleDrawInLoopUntilInputIsMade()
    {
        var topOffset = Console.CursorTop;
        var bottomOffset = 0;
        Console.CursorVisible = false;


        while (!_itemIsSelected)
        {
            for (var i = 0; i < _menuItems.Length; i++)
            {
                WriteConsoleItem(i, _selectedItemIndex);
            }

            bottomOffset = Console.CursorTop;
            var kb = Console.ReadKey(true);
            HandleKeyPress(kb.Key);

            //Reset the cursor to the top of the screen
            Console.SetCursorPosition(0, topOffset);
        }

        //set the cursor just after the menu so that the program can continue after the menu
        Console.SetCursorPosition(0, bottomOffset);
        Console.CursorVisible = true;
    }

    private void HandleKeyPress(ConsoleKey pressedKey)
    {
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (pressedKey)
        {
            case ConsoleKey.UpArrow:
                _selectedItemIndex = (_selectedItemIndex == 0) ? _menuItems.Length - 1 : _selectedItemIndex - 1;
                CheckForUnselectable(pressedKey);
                break;

            case ConsoleKey.DownArrow:
                _selectedItemIndex = (_selectedItemIndex == _menuItems.Length - 1) ? 0 : _selectedItemIndex + 1;
                CheckForUnselectable(pressedKey);
                break;

            case ConsoleKey.Enter:
                _itemIsSelected = true;
                break;
        }
    }
    private void CheckForUnselectable(ConsoleKey pressedKey)
    {
        if (_menuItems[_selectedItemIndex].GetType() == typeof(ConsoleMenuSeparator))
        {
            HandleKeyPress(pressedKey);
        }
    }

    private void WriteConsoleItem(int itemIndex, int selectedItemIndex)
    {
        if (itemIndex == selectedItemIndex)
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
        }

        var text = _menuItems[itemIndex].Label;
        if (_menuItems[itemIndex].GetType() == typeof(ConsoleMenuSeparator))
        {
            text = text.PadRight(_menuItems.Max(x => x.Label.Length), _menuItems[itemIndex].Label[0]);
        }
        Console.WriteLine(" {0,-20}", text);
        Console.ResetColor();
    }
}

public abstract class ConsoleMenuItem
{
    protected ConsoleMenuItem(string label)
    {
        Label = label;
    }
    public string Label { get; }
    public virtual void Execute() { }
    public virtual Task ExecuteAsync() => Task.CompletedTask;
}

public class ConsoleMenuItem<T> : ConsoleMenuItem
{
    private readonly Action<T> _callBack;
    private readonly Func<T, Task> _asyncCallBack;
    private readonly T _underlyingObject;

    public ConsoleMenuItem(string label, Action<T> callback, T underlyingObject = default)
        : base(label)
    {
        _callBack = callback;
        _underlyingObject = underlyingObject;
    }

    public ConsoleMenuItem(string label, Func<T, Task> callback, T underlyingObject = default)
        : base(label)
    {
        _asyncCallBack = callback;
        _underlyingObject = underlyingObject;
    }

    public override void Execute()
    {
        _callBack?.Invoke(_underlyingObject);
        _asyncCallBack?.Invoke(_underlyingObject).GetAwaiter().GetResult();
    }

    public override async Task ExecuteAsync()
    {
        _callBack?.Invoke(_underlyingObject);
        if (_asyncCallBack is not null)
            await _asyncCallBack.Invoke(_underlyingObject);
    }
}

public class ConsoleMenuSeparator : ConsoleMenuItem
{
    public ConsoleMenuSeparator(char separatorChar = '-')
        : base(separatorChar.ToString())
    { }
}