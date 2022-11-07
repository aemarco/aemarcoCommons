using aemarcoCommons.Extensions.AttributeExtensions;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;

namespace aemarcoCommons.WpfTools.BaseModels;

//Github: https://github.com/CommunityToolkit/dotnet
//Docs: https://learn.microsoft.com/de-de/dotnet/communitytoolkit/mvvm/
//Samples Github: https://github.com/CommunityToolkit/MVVM-Samples
//Samples AppStore: https://apps.microsoft.com/store/detail/mvvm-toolkit-sample-app/9NKLCF1LVZ5H?hl=en-us&gl=us
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




        //*** To be remove when NotifyTriggersEventAttribute being removed ***
        //*** To be remove when NotifyTriggersEventAttribute being removed ***
        //*** To be remove when NotifyTriggersEventAttribute being removed ***


        //NotifyTriggersEventAttribute
#pragma warning disable CS0618
        foreach (var eventAttribute in this.GetAttributes<NotifyTriggersEventAttribute>(e.PropertyName))
#pragma warning restore CS0618
        {
            if (GetType()
                    .GetField(eventAttribute.EventName, BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.GetValue(this)
                is not MulticastDelegate multiDelegate)
                continue;

            foreach (var eventHandler in multiDelegate.GetInvocationList())
            {
                eventHandler.Method.Invoke(
                    eventHandler.Target,
                    new object[] { this, EventArgs.Empty });
            }
        }

        //*** To be remove when NotifyTriggersEventAttribute being removed ***
        //*** To be remove when NotifyTriggersEventAttribute being removed ***
        //*** To be remove when NotifyTriggersEventAttribute being removed ***
    }


    [Obsolete("Use OnPropertyChanged instead")]
    protected virtual void NotifyPropertyChanged(
        [CallerMemberName] string propertyName = null) => OnPropertyChanged(propertyName);


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



