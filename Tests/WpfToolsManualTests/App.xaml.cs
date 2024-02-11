using aemarcoCommons.WpfTools.WindowStuff;
using System.Windows;
using WpfToolsManualTests.View;
using WpfToolsManualTests.ViewModel;

namespace WpfToolsManualTests;

public partial class App
{
    private void App_OnStartup(object sender, StartupEventArgs e)
    {
        IWindowService.RegisterView<TestWindow, TestViewModel>();
        IWindowService.RegisterDialog<TestManualDialogView, TestManualDialogViewModel>();

        var windowService = new WindowService();

        MainWindow = new MainWindow()
        {
            DataContext = new MainWindowViewModel(windowService)
        };
        MainWindow.Show();
    }
}