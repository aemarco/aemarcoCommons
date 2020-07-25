using System;

namespace WpfTools.BaseModels
{
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
}
