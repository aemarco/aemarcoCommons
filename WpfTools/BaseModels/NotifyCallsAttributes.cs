using System;

namespace aemarcoCommons.WpfTools.BaseModels
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


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class NotifyTriggersEventAttribute : Attribute
    {
        public NotifyTriggersEventAttribute(string eventName)
        {
           
            EventName = eventName;
            
        }
        public string EventName { get; }
    }
}
