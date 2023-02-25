using System.Windows;

namespace WpfToolsManualTests.View;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {

        MessageBox.Show("Yes");
    }
}