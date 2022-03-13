using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace aemarcoCommons.Extensions.AttributeExtensions
{
    public static class ReadingExtensions
    {

        public static T GetAttribute<T>(this Type type, string memberName = null) 
            where T : Attribute
        {
            //get attribute on type
            if (memberName == null)
            {
                return (T)type.GetCustomAttribute(typeof(T));
            }
            //get attribute on member
            else
            {
                return (T) type.GetMember(memberName).FirstOrDefault()?.GetCustomAttribute(typeof(T));
            }
        }

        public static T GetAttribute<T>(this object obj, string memberName = null) 
            where T : Attribute
        {
            return obj.GetType().GetAttribute<T>(memberName);
        }

        public static bool HasAttribute<T>(this Type type, string memberName = null)
            where T : Attribute
        {
            return type.GetAttribute<T>(memberName) != null;
        }

        public static bool HasAttribute<T>(this object obj, string memberName = null)
            where T : Attribute
        {
            return obj.GetAttribute<T>(memberName) != null;
        }




        public static IEnumerable<T> GetAttributes<T>(this Type type, string memberName = null) 
            where T : Attribute
        {
            //get attribute on type
            if (memberName == null)
            {
                return type
                    .GetCustomAttributes(typeof(T))
                    .OfType<T>();
            }
            //get attribute on member
            else
            {
                return type
                    .GetMember(memberName)
                    .FirstOrDefault()?
                    .GetCustomAttributes(typeof(T))
                    .OfType<T>() ?? new List<T>(); // ?? because we want a collection also if there is no member found
            }
        }

        public static IEnumerable<T> GetAttributes<T>(this object obj, string memberName = null) 
            where T : Attribute
        {
            return obj.GetType().GetAttributes<T>(memberName);
        }


        public static T GetAttribute<T>(this Enum value)
            where T : Attribute
        {
            var enumMember = value.GetType().GetMember(value.ToString()).First();
            var attributes = enumMember.GetCustomAttributes(typeof(T), false).Cast<T>();
            return attributes.FirstOrDefault();
        }

    }
}
