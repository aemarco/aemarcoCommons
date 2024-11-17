using aemarcoCommons.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;

namespace aemarcoCommons.WpfTools.BaseModels;

//Github: https://github.com/CommunityToolkit/dotnet
//Docs: https://learn.microsoft.com/de-de/dotnet/communitytoolkit/mvvm/
//Samples Github: https://github.com/CommunityToolkit/MVVM-Samples
//Samples AppStore: https://apps.microsoft.com/store/detail/mvvm-toolkit-sample-app/9NKLCF1LVZ5H?hl=en-us&gl=us

//Announcing 8.0 https://devblogs.microsoft.com/dotnet/announcing-the-dotnet-community-toolkit-800/
//Announcing 8.1 https://devblogs.microsoft.com/dotnet/announcing-the-dotnet-community-toolkit-810/
//Announcing 8.2 https://devblogs.microsoft.com/dotnet/announcing-the-dotnet-community-toolkit-820/

public class BaseNotifier : ObservableObject
{

    private readonly Dispatcher _dispatcher;
    protected BaseNotifier()
    {
        _dispatcher = Dispatcher.CurrentDispatcher;
    }
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            base.OnPropertyChanged(e);
        else
            _dispatcher.Invoke(() => base.OnPropertyChanged(e));


        //NotifyCallsMethodAttribute
        foreach (var methodAttribute in this.GetAttributes<NotifyCallsMethodAttribute>(e.PropertyName))
        {
            GetType().GetMethod(methodAttribute.MethodName, BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(this, methodAttribute.Parameters);
        }

    }

}



[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class NotifyCallsMethodAttribute : Attribute
{
    public NotifyCallsMethodAttribute(string methodName, params object[] parameters)
    {

        MethodName = methodName;
        Parameters = parameters;
    }
    public string MethodName { get; }
    public object[] Parameters { get; }
}



