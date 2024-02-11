using aemarcoCommons.WpfTools.Commands;
using System;
using System.Windows;
using System.Windows.Input;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.WpfTools.Dialogs;

// ReSharper disable once RedundantExtendsListEntry


[Obsolete]
public partial class InputDialog : Window
{
    public InputDialog(string question, string defaultAnswer = "")
    {
        Question = question;
        _answer = defaultAnswer;

        InitializeComponent();
        DataContext = this;
    }

    public string Question { get; }

    private string _answer;
    public string Answer
    {
        get => _answer;
        set
        {
            if (_answer.Equals(value))
                return;

            _answer = value;
        }
    }


    public ICommand OkayCommand =>
        new DelegateCommand
        {
            CommandAction = _ =>
            {
                DialogResult = true;
            }
        };


}