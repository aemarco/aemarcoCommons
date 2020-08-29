using System;
using System.Collections.Generic;
using System.Text;

namespace WpfTools.BaseNav
{
    //windows
    public interface INavMainWindow : INavWindow { } //implement this in the main window

    //view models
    public interface INavMainViewModel : INavWindowViewModel { } //implement this in the main view model, inherit BaseNavWindowViewModel

}
