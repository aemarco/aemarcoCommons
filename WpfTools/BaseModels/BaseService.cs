using System.Reflection;
using System.Runtime.CompilerServices;
using Extensions.AttributeExtensions;

namespace WpfTools.BaseModels
{
    public class BaseService : BaseNotifier, IBaseService
    {
        protected override void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
        {
            base.NotifyPropertyChanged(propertyName);
            foreach (var methodAttribute in this.GetAttributes<NotifyCallsMethodAttribute>(propertyName))
            {
                GetType().GetMethod(methodAttribute.MethodName, BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.Invoke(this, methodAttribute.Parameters);
            }
        }

    }
}
