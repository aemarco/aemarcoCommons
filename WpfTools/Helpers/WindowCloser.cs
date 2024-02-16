using System;
using System.ComponentModel;
using System.Windows;

namespace aemarcoCommons.WpfTools.Helpers;

//partially inspired by
//https://youtu.be/U7Qclpe2joo


public interface ICloseWindow
{
    Action CloseAction { set; }
    bool CanClose { get; }
}



/// <summary>
/// Can be attached to a window, so it can be closed by the view model (probably inherit BaseViewModel)
/// </summary>
public class WindowCloser
{
    public static bool? GetEnableWindowClosing(DependencyObject obj) => (bool?)obj.GetValue(EnableWindowClosingProperty);
    public static void SetEnableWindowClosing(DependencyObject obj, bool? value) => obj.SetValue(EnableWindowClosingProperty, value);

    public static readonly DependencyProperty EnableWindowClosingProperty = DependencyProperty.RegisterAttached(
        "EnableWindowClosing",
        typeof(bool?),
        typeof(WindowCloser),
        new PropertyMetadata(null, OnEnableWindowClosingChanged));


    private static void OnEnableWindowClosingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Window { DataContext: ICloseWindow windowCloser } window)
            return;

        window.Loaded += WindowLoadedEventHandler;
        window.Closing += ClosingEventHandler;

        return;

        void WindowLoadedEventHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            windowCloser.CloseAction = () => window.Close();
        }

        void ClosingEventHandler(object sender, CancelEventArgs cancelArgs)
        {
            if (!windowCloser.CanClose)
            {
                cancelArgs.Cancel = true;
                return;
            }

            window.Loaded -= WindowLoadedEventHandler;
            window.Closing -= ClosingEventHandler;


            // ReSharper disable once SuspiciousTypeConversion.Global
            if (windowCloser is IDisposable dis)
                dis.Dispose();
        }

    }
}