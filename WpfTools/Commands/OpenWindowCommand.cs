using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using WpfTools.BaseModels;

namespace WpfTools.Commands
{
    public class OpenWindowCommand<T> : DelegateCommand where T : IWindow
    {
        public OpenWindowCommand(T window)
        {
            CommandAction = window.Show;
        }
    }
}
