using System;
using System.Linq;

namespace Extensions.netExtensions
{
    public static class AttributeExtensions
    {
        public static T GetAttribute<T>(this Enum value)
            where T : Attribute
        {
            var enumMember = value.GetType().GetMember(value.ToString()).First();
            var attributes = enumMember.GetCustomAttributes(typeof(T), false).Cast<T>();
            return attributes.FirstOrDefault();
        }



    }
}
