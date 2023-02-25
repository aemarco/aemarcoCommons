using aemarcoCommons.WpfTools.Commands;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace aemarcoCommons.WpfTools.Dialogs;


public partial class LoginDialog : ILoginDialog, ITransient, INotifyPropertyChanged
{
    private readonly LoginActionProvider _loginActionProvider;

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion


    public LoginDialog(LoginActionProvider loginActionProvider)
    {
        _loginActionProvider = loginActionProvider;
        Username = loginActionProvider.DefaultUsername;

        InitializeComponent();
        DataContext = this;
    }

    public string Username { get; set; }
    public string LoginButtonText { get; set; } = "Login";


    private bool _currentlyLoggingIn;
    public ICommand LoginCommand =>
        new AsyncDelegateCommand
        {
            CanExecuteFunc = _ =>
                !string.IsNullOrWhiteSpace(Username) &&
                !string.IsNullOrWhiteSpace(PasswordBox.Password) &&
                !_currentlyLoggingIn,
            CommandAction = async _ =>
            {
                _currentlyLoggingIn = true;
                NotifyPropertyChanged();

                if (await _loginActionProvider.LoginAction(Username, PasswordBox.Password))
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    LoginButtonText = "Login failed. Try again";
                    NotifyPropertyChanged(nameof(LoginButtonText));
                }

                _currentlyLoggingIn = false;
                NotifyPropertyChanged();
            }
        };
}

public class LoginActionProvider
{
    public string DefaultUsername { get; set; }
    public Func<string, string, Task<bool>> LoginAction { get; set; }
}