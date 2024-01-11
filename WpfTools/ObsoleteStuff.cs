using System;
using System.Threading.Tasks;
using System.Windows;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.WpfTools.BaseModels
{
    [Obsolete("Use BaseNotifier instead")]
    public class BaseService : BaseNotifier;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    [Obsolete("Use NotifyCallsMethodAttribute instead")]
    //when removing this, remove also code checking for in BaseNotifier !!!!
    public class NotifyTriggersEventAttribute : Attribute
    {
        public NotifyTriggersEventAttribute(string eventName)
        {
            EventName = eventName;
        }
        public string EventName { get; }

    }
}


namespace aemarcoCommons.WpfTools.Commands
{
    [Obsolete]
    public class ExitApplicationCommand : AsyncDelegateCommand
    {
        public ExitApplicationCommand(
            Func<Task> beforeClose = null)
        {
            CommandAction = async _ =>
            {
                if (beforeClose is not null)
                    await beforeClose();
                Application.Current.Shutdown();
            };
        }
    }
}