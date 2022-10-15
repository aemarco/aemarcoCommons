using System.Windows;
using WpfToolsManualTests.View;
using WpfToolsManualTests.ViewModel;

namespace WpfToolsManualTests;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        MainWindow = new MainWindow()
        {
            DataContext = new MainWindowViewModel()
        };
        MainWindow.Show();

        base.OnStartup(e);
    }
}