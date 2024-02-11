using aemarcoCommons.WpfTools.BaseModels;
using aemarcoCommons.WpfTools.Dialogs;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace aemarcoCommons.WpfTools.WindowStuff;

//inspired by
//https://youtu.be/S8hEjLahNtU

public interface IWindowService
{

    void Show<TViewModel>() where TViewModel : BaseViewModel;

    bool? ShowDialog(DialogViewModel viewModel);
    bool? ShowDialog<TViewModel>() where TViewModel : DialogViewModel;



    static readonly Dictionary<Type, Type> Mappings = [];
    static void RegisterView<TWindow, TViewModel>()
        where TWindow : Window, new()
        where TViewModel : BaseViewModel
    {
        Mappings[typeof(TViewModel)] = typeof(TWindow);
    }
    static void RegisterDialog<TView, TViewModel>()
        where TView : UserControl, new()
        where TViewModel : DialogViewModel
    {
        Mappings.Add(typeof(TViewModel), typeof(TView));
    }

}