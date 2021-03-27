using aemarcoCommons.Extensions.AttributeExtensions;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace aemarcoCommons.WpfTools.BaseModels
{
    public class BaseService : BaseNotifier, IBaseService
    {
        protected override void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.NotifyPropertyChanged(propertyName);


            //NotifyCallsMethodAttribute
            foreach (var methodAttribute in this.GetAttributes<NotifyCallsMethodAttribute>(propertyName))
            {
                GetType().GetMethod(methodAttribute.MethodName, BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.Invoke(this, methodAttribute.Parameters);
            }

            //NotifyTriggersEventAttribute
            foreach (var eventAttribute in this.GetAttributes<NotifyTriggersEventAttribute>(propertyName))
            {
                if (!(GetType()
                    .GetField(eventAttribute.EventName, BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.GetValue(this) is MulticastDelegate multiDelegate)) continue;

                foreach (var eventHandler in multiDelegate.GetInvocationList())
                {
                    eventHandler.Method.Invoke(
                        eventHandler.Target,
                        new object[] { this, EventArgs.Empty });
                }
            }

        }

    }
}
