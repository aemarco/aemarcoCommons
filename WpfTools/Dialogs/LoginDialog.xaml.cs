using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using aemarcoCommons.WpfTools.Commands;

namespace aemarcoCommons.WpfTools.Dialogs
{
    /// <summary>
    /// Interaktionslogik für LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window, ILoginDialog, ITransient, INotifyPropertyChanged
    {
        private readonly LoginActionProvider _loginActionProvider;

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
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
        public ICommand LoginCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => 
                        !string.IsNullOrWhiteSpace(Username) && 
                        !string.IsNullOrWhiteSpace(PasswordBox.Password) &&
                        !_currentlyLoggingIn,
                    CommandAction = async () =>
                    {
                        _currentlyLoggingIn = true;
                        NotifyPropertyChanged(nameof(LoginCommand));
                        
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
                        NotifyPropertyChanged(nameof(LoginCommand));
                    }
                };
            }
        }
    }

    public class LoginActionProvider
    {
        public string DefaultUsername { get; set; }
        public Func<string, string, Task<bool>> LoginAction { get; set; }
    }
    
}
