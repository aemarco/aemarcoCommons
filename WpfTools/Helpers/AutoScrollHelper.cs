using System;
using System.Windows;
using System.Windows.Controls;

namespace aemarcoCommons.WpfTools.Helpers;

public enum AutoScrollHelperMode
{
    None,
    ExtentHeightChangeToBottom,
    ExtendHeightChangeToTop,
    ExtendWidthChangeToRight,
    ExtendWidthChangeToLeft
}



public static class AutoScrollHelper
{
    public static AutoScrollHelperMode GetAutoScroll(DependencyObject obj)
    {
        return (AutoScrollHelperMode)obj.GetValue(AutoScrollProperty);
    }

    public static void SetAutoScroll(DependencyObject obj, AutoScrollHelperMode value)
    {
        obj.SetValue(AutoScrollProperty, value);
    }

    public static readonly DependencyProperty AutoScrollProperty =
        DependencyProperty.RegisterAttached(
            "AutoScroll",
            typeof(AutoScrollHelperMode),
            typeof(AutoScrollHelper),
            new PropertyMetadata(AutoScrollHelperMode.None, AutoScrollPropertyChanged));

    private static void AutoScrollPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        //ensure scroll viewer
        if (!(d is ScrollViewer scrollViewer)) return;


        //ensure it set only one time
        if ((AutoScrollHelperMode)e.OldValue != AutoScrollHelperMode.None)
            throw new ApplicationException("Cant change scroll mode");

        //var setValue = (AutoScrollHelperMode)e.NewValue;
        scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
    }

    private static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {

        var setValue = GetAutoScroll((DependencyObject)sender);
        switch (setValue)
        {
            case AutoScrollHelperMode.None:
                return;
            case AutoScrollHelperMode.ExtentHeightChangeToBottom:
                {
                    if (Math.Abs(e.ExtentHeightChange) > 0) ((ScrollViewer)sender).ScrollToBottom();
                    return;
                }
            case AutoScrollHelperMode.ExtendHeightChangeToTop:
                {
                    if (Math.Abs(e.ExtentHeightChange) > 0) ((ScrollViewer)sender).ScrollToTop();
                    return;
                }
            case AutoScrollHelperMode.ExtendWidthChangeToRight:
                {
                    if (Math.Abs(e.ExtentWidthChange) > 0) ((ScrollViewer)sender).ScrollToRightEnd();
                    return;
                }
            case AutoScrollHelperMode.ExtendWidthChangeToLeft:
                {
                    if (Math.Abs(e.ExtentWidthChange) > 0) ((ScrollViewer)sender).ScrollToLeftEnd();
                    return;
                }
            default:
                throw new NotImplementedException();
        }




    }
}