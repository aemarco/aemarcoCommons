using aemarcoCommons.WpfTools.BaseModels;
using aemarcoCommons.WpfTools.Commands;
using Autofac;
using Autofac.Core;
using System;
using System.Windows;

namespace aemarcoCommons.WpfTools.BaseNav
{
    public abstract class BaseNavWindowViewModel : BaseViewModel //inherit this in window view model
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="window">window which uses this view model</param>
        protected BaseNavWindowViewModel(Window window)
        {
            Window = window;
            Window.DataContext = this;
        }

        //so that we can access the window belonging to this view model
        public Window Window { get; set; }

        //so that we can navigate
        public void ShowViewFor<T>(params Parameter[] p) where T : INavViewModel
        {
            ViewViewModel = Resolve<T>(p);
            //set reference so that navigation view models can access this window view model
            ViewViewModel.WindowViewModel = this;

            //Update View so navigation takes place
            View = ViewViewModel.View;


            NotifyPropertyChanged(nameof(ViewViewModel));
            NotifyPropertyChanged(nameof(View));
            NotifyPropertyChanged(nameof(Title));

        }

        public INavViewModel ViewViewModel { get; set; }

        public virtual string Title => ViewViewModel?.Title ?? GetType().Name;

        /// <summary>
        /// Bind this to get the navigation view
        /// </summary>
        public INavView View { get; set; }


        /// <summary>
        /// Resolve requested INavViewModel. Override if DiExtension is not used!
        /// </summary>
        /// <param name="p">Parameters To Pass to the Constructor</param>
        /// <typeparam name="T">Interface of view model to resolve</typeparam>
        /// <returns>requested view model</returns>
        protected virtual T Resolve<T>(params Parameter[] p) where T : INavViewModel => Bootstrapper.RootScope != null
                ? Bootstrapper.RootScope.Resolve<T>(p)
                : throw new NotImplementedException("Override Resolve to resolve INavViewModel´s");


        public override DelegateCommand CloseCommand =>
            new()
            {
                CommandAction = _ =>
                {
                    Window?.Close();
                }
            };
    }
}